using Bifröst.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Bifröst
{

    public sealed class Bus : IBus, IMetrics, IDisposable
    {
        private readonly TimeSpan cancellationTimeout = TimeSpan.FromMilliseconds(10);
        private readonly AsyncAutoResetEvent runEvent = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent idleEvent = new AsyncAutoResetEvent(false);

        private long receivedEventsCount = 0;
        private long processedEventCount = 0;

        private readonly Channel<IEvent> incomingChannel = Channel.CreateUnbounded<IEvent>();
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();

        private readonly Dictionary<Guid, IList<IEvent>> failedEvents = new Dictionary<Guid, IList<IEvent>>();

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private bool isDisposing = false;

        public bool IsRunning { get; private set; }

        public void Subscribe(ISubscription subscription)
        {
            if (subscription is null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            this.subscriptions.Add(subscription);
        }

        public void Unsubscribe(ISubscription subscription)
        {
            if (subscription is null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            this.subscriptions.Remove(subscription);
        }

        public async Task WriteAsync(IEvent evt)
        {
            await this.incomingChannel.Writer.WriteAsync(evt)
                .ConfigureAwait(false);

            this.receivedEventsCount++;
        }

        public async Task RunAsync()
        {
            this.tokenSource = new CancellationTokenSource();

            _ = Task.Run(() => this.ProcessAsync());

            await this.runEvent.WaitAsync(TimeSpan.FromMilliseconds(10))
                .ConfigureAwait(false);
        }

        public async Task IdleAsync()
        {
            this.tokenSource.Cancel();

            await this.idleEvent.WaitAsync(TimeSpan.FromMilliseconds(10))
                .ConfigureAwait(false);
        }

        private async Task ProcessAsync()
        {
            try
            {
                this.IsRunning = true;
                this.runEvent.Set();

                await foreach (var evt in this.incomingChannel.Reader.ReadAllAsync(this.tokenSource.Token))
                {
                    var matchedSubscriptions = this.subscriptions.Where(s => s.Matches(evt.Topic));

                    var forwardingTasks = new List<Task>();

                    foreach (var subscriber in matchedSubscriptions)
                    {
                        using var cancellationTokenSource = new CancellationTokenSource(this.cancellationTimeout);

                        try
                        {
                            await subscriber.WriteAsync(evt, cancellationTokenSource.Token)
                                    .ConfigureAwait(false);
                        }
                        catch (TaskCanceledException)
                        {
                            if(this.failedEvents.ContainsKey(subscriber.Id))
                            {
                                this.failedEvents.Add(subscriber.Id, new List<IEvent>{ evt });
                            }
                            else
                            {
                                this.failedEvents[subscriber.Id].Add(evt);
                            }
                        }
                    }

                    this.processedEventCount++;
                }
            }
            finally
            {
                this.IsRunning = false;
                this.idleEvent.Set();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!this.isDisposing)
            {
                if (disposing)
                {
                    this.tokenSource.Dispose();
                }

                this.isDisposing = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public IEnumerable<Metric> GetMetrics()
        {
            yield return new Metric(Metrics.Bus.ReceivedEvents, this.receivedEventsCount);
            yield return new Metric(Metrics.Bus.ProcessedEvents, this.processedEventCount);
            yield return new Metric(Metrics.Bus.IsRunning, this.IsRunning);
        }
    }
}