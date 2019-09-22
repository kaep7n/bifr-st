using Bifröst.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bifröst
{
    public sealed class Bus : IDisposable
    {
        private readonly AsyncQueue<IEvent> eventsQueue = new AsyncQueue<IEvent>();
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();

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

        public async Task EnqueueAsync(IEvent evt)
        {
            await this.eventsQueue.EnqueueAsync(evt)
                .ConfigureAwait(false);
        }

        public void Start()
        {
            this.tokenSource = new CancellationTokenSource();

            Task.Run(() => this.ProcessAsync());
        }

        public void Stop()
        {
            this.tokenSource.Cancel();
        }

        private async Task ProcessAsync()
        {
            try
            {
                this.IsRunning = true;

                await foreach (var evt in this.eventsQueue.WithCancellation(this.tokenSource.Token))
                {
                    this.subscriptions
                        .Where(s => s.Matches(evt.Topic))
                        .ForEach(async s => await s.EnqueueAsync(evt).ConfigureAwait(false));
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
                    this.eventsQueue.Dispose();
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
