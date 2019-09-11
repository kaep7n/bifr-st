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
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly AsyncQueue<IEvent> eventsQueue = new AsyncQueue<IEvent>();
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();

        private bool isDisposing = false;

        public bool IsRunning { get; private set; }

        public void Subscribe(ISubscription subscription)
        {
            this.subscriptions.Add(subscription);
        }

        public void Unsubscribe(ISubscription subscription)
        {
            this.subscriptions.Remove(subscription);
        }

        public async Task EnqueueAsync(IEvent evt)
        {
            await this.eventsQueue.EnqueueAsync(evt)
                .ConfigureAwait(false);
        }

        public void Start()
        {
            Task.Run(() => this.ProcessAsync());
            this.IsRunning = true;
        }

        public void Stop()
        {
            this.tokenSource.Cancel();
            this.IsRunning = false;
        }

        private async Task ProcessAsync()
        {
            await foreach (var evt in this.eventsQueue
                .WithCancellation(this.tokenSource.Token))
            {
                this.subscriptions
                .Where(s => s.Matches(evt.Topic))
                .ForEach(s => s.Receive(evt));
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
