using Bifröst.Extensions;
using Bifröst.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Bifröst
{
    public sealed class Bus : IBus, IDisposable
    {
        private long writtenEventCount = 0;
        private long processedEventCount = 0;
        private long failedEventCount = 0;

        private readonly Channel<IEvent> incomingChannel = Channel.CreateUnbounded<IEvent>();
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private bool isDisposing = false;

        public bool IsRunning { get; private set; }

        public long WaitingEventCount => this.writtenEventCount - this.processedEventCount;

        public long ProcessedEventCount => this.processedEventCount;

        public long FailedEventCount => this.failedEventCount;

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

            this.writtenEventCount++;
        }

        public void Run()
        {
            this.tokenSource = new CancellationTokenSource();

            Task.Run(async () => await this.ProcessAsync().ConfigureAwait(false));
        }

        public void Idle()
        {
            this.tokenSource.Cancel();
        }

        private async Task ProcessAsync()
        {
            try
            {
                this.IsRunning = true;

                await foreach (var evt in this.incomingChannel.Reader.ReadAllAsync(this.tokenSource.Token))
                {
                    var matchedSubscriptions = this.subscriptions.Where(s => s.Matches(evt.Topic));

                    foreach (var sub in matchedSubscriptions)
                    {
                        await sub.WriteAsync(evt).ConfigureAwait(false);
                    }

                    this.processedEventCount++;
                }
            }
            finally
            {
                this.IsRunning = false;
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
    }
}
