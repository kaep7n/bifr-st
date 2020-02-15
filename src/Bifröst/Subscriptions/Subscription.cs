using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Bifröst.Subscriptions
{
    public abstract class Subscription : IDisposable, ISubscription
    {
        private readonly Channel<IEvent> incomingChannel = Channel.CreateUnbounded<IEvent>();
        private readonly IBus bus;
        private CancellationTokenSource tokenSource;
        private bool isDisposing = false;

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

        public bool Matches(Topic topic)
        {
            return this.Pattern.Matches(topic);
        }

        public async Task WriteAsync(IEvent evt)
        {
            if (evt is null)
            {
                throw new ArgumentNullException(nameof(evt));
            }

            await this.incomingChannel.Writer
                .WriteAsync(evt)
                .ConfigureAwait(false);
        }

        public void Enable()
        {
            this.tokenSource = new CancellationTokenSource();

            this.bus.Subscribe(this);

            Task.Run(() => this.ProcessInput());
        }

        protected abstract Task ProcessEventAsync(IEvent evt);

        private async Task ProcessInput()
        {
            try
            {
                this.IsEnabled = true;

                await foreach (var evt in this.incomingChannel.Reader
                    .ReadAllAsync()
                    .WithCancellation(this.tokenSource.Token))
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
            this.bus.Unsubscribe(this);
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
                }

                this.isDisposing = true;
            }
        }
    }
}
