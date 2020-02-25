using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Bifröst.Subscriptions
{
    public abstract class Subscription : ISubscription, IMetrics, IDisposable
    {
        private readonly Channel<IEvent> incomingChannel = Channel.CreateUnbounded<IEvent>();
        private readonly IBus bus;

        private readonly AsyncAutoResetEvent enableEvent = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent disableEvent = new AsyncAutoResetEvent(false);

        private CancellationTokenSource tokenSource;
        private bool isDisposing = false;

        private long receivedEvents = 0;
        private long processedEvents = 0;

        public Subscription(IBus bus, Pattern pattern)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            if (pattern is null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            this.Id = Guid.NewGuid();
            this.bus = bus;
            this.Pattern = pattern;
        }

        public Guid Id { get; }

        public Pattern Pattern { get; }

        public bool IsEnabled { get; private set; }

        public bool Matches(Topic topic) => this.Pattern.Matches(topic);

        public async Task WriteAsync(IEvent evt, CancellationToken cancellationToken = default)
        {
            if (evt is null)
            {
                throw new ArgumentNullException(nameof(evt));
            }

            await this.incomingChannel.Writer
                .WriteAsync(evt, cancellationToken)
                .ConfigureAwait(false);

            this.receivedEvents++;
        }

        public async Task EnableAsync(CancellationToken cancellationToken = default)
        {
            this.tokenSource = new CancellationTokenSource();

            this.bus.Subscribe(this);

            _ = Task.Run(() => this.ProcessInput());

            await this.enableEvent.WaitAsync(TimeSpan.FromMilliseconds(10), cancellationToken)
                .ConfigureAwait(false);
        }

        protected abstract Task ProcessEventAsync(IEvent evt);

        private async Task ProcessInput()
        {
            try
            {
                this.IsEnabled = true;
                this.enableEvent.Set();

                await foreach (var evt in this.incomingChannel.Reader.ReadAllAsync(this.tokenSource.Token))
                {
                    await this.ProcessEventAsync(evt)
                        .ConfigureAwait(false);

                    this.processedEvents++;
                }
            }
            finally
            {
                this.IsEnabled = false;
            }
        }

        public async Task DisableAsync(CancellationToken cancellationToken = default)
        {
            this.bus.Unsubscribe(this);
            this.tokenSource.Cancel();

            await this.disableEvent.WaitAsync(TimeSpan.FromMilliseconds(10), cancellationToken)
                .ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposing)
            {
                if (disposing)
                {
                    this.tokenSource?.Dispose();
                }

                this.isDisposing = true;
            }
        }

        public virtual IEnumerable<Metric> GetMetrics()
        {
            yield return new Metric(Metrics.Subscription.ReceivedEvents, this.receivedEvents);
            yield return new Metric(Metrics.Subscription.ProcessedEvents, this.processedEvents);
            yield return new Metric(Metrics.Subscription.IsEnabled, this.IsEnabled);
        }
    }
}
