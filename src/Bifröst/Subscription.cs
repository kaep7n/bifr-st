using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bifröst
{
    public abstract class Subscription : IDisposable, ISubscription
    {
        private readonly AsyncQueue<IEvent> incomingQueue = new AsyncQueue<IEvent>();

        private CancellationTokenSource tokenSource;
        private bool isDisposing = false;

        public Subscription(params Topic[] topics)
        {
            if (topics is null)
            {
                throw new ArgumentNullException(nameof(topics));
            }

            this.Id = Guid.NewGuid();
            this.Topics = topics;
        }

        public Guid Id { get; }

        public IEnumerable<Topic> Topics { get; }

        public bool IsEnabled { get; private set; }

        public bool Matches(Topic topic)
        {
            return this.Topics.Any(t => t.Equals(topic));
        }

        public async Task EnqueueAsync(IEvent evt)
        {
            if (evt is null)
            {
                throw new ArgumentNullException(nameof(evt));
            }

            await this.incomingQueue.EnqueueAsync(evt)
                .ConfigureAwait(false);
        }

        public void Enable()
        {
            this.tokenSource = new CancellationTokenSource();

            Task.Run(() => this.ProcessIncomingQueueAsync());
        }

        protected abstract Task ProcessEventAsync(IEvent evt);

        private async Task ProcessIncomingQueueAsync()
        {
            try
            {
                this.IsEnabled = true;

                await foreach (var evt in this.incomingQueue.WithCancellation(this.tokenSource.Token))
                {
                    await this.ProcessEventAsync(evt)
                        .ConfigureAwait(false);
                }
            }
            finally
            {
                this.IsEnabled = false;
            }
        }

        public void Disable()
        {
            this.tokenSource.Cancel();
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
                    this.tokenSource.Dispose();
                    this.incomingQueue.Dispose();
                }

                this.isDisposing = true;
            }
        }
    }
}
