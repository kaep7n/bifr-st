using Bifröst.Subscriptions;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bifröst.Tests.Resources
{
    public class FakeSubscription : ISubscription
    {
        private readonly AsyncAutoResetEvent waitUntilWrite = new AsyncAutoResetEvent(false);
        private readonly List<IEvent> receivedEvents = new List<IEvent>();
        
        private bool block;

        public FakeSubscription(Pattern pattern, bool block = false)
        {
            if (pattern is null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            this.Id = Guid.NewGuid();
            this.Pattern = pattern;
            this.block = block;
        }

        public Guid Id { get; }

        public bool IsEnabled { get; private set; }

        public Pattern Pattern { get; }

        public IEnumerable<IEvent> ReceivedEvents => this.receivedEvents;

        public Task EnableAsync(CancellationToken cancellationToken = default)
        {
            this.IsEnabled = true;
            return Task.CompletedTask;
        }

        public Task DisableAsync(CancellationToken cancellationToken = default)
        {
            this.IsEnabled = false;
            return Task.CompletedTask;
        }

        public bool Matches(Topic topic)
            => this.Pattern.Matches(topic);

        public async Task<bool> WaitUntilWrite(TimeSpan timeout)
        {
            using var cancellationTokenSource = new CancellationTokenSource(timeout);

            try
            {
                await this.waitUntilWrite.WaitAsync(cancellationTokenSource.Token)
                    .ConfigureAwait(false);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        public void BlockWrite()
        {
            this.block = true;
        }

        public void UnBlockWrite()
        {
            this.block = false;
        }

        public async Task WriteAsync(IEvent evt, CancellationToken cancellationToken = default)
        {
            if (this.block)
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(false);
            }

            this.receivedEvents.Add(evt);
            this.waitUntilWrite.Set();
        }
    }
}
