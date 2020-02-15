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
        private readonly Channel<IEvent> incomingChannel = Channel.CreateUnbounded<IEvent>();
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
            await this.incomingChannel.Writer.WriteAsync(evt)
                .ConfigureAwait(false);
        }

        public void Run()
        {
            this.tokenSource = new CancellationTokenSource();

            Task.Run(() => this.ProcessAsync());
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

                await foreach (var evt in this.incomingChannel.Reader
                    .ReadAllAsync()
                    .WithCancellation(this.tokenSource.Token))
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
