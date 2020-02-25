using Bifröst.Subscriptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bifröst.Tests.Resources
{
    public class FakeSubscription : ISubscription
    {
        private readonly AsyncAutoResetEvent waitBeforeWrite = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent waitUntilWrite = new AsyncAutoResetEvent(false);
        private readonly List<IEvent> receivedEvents = new List<IEvent>();
        private readonly TimeSpan waitBeforeWriteTimeout;

        public FakeSubscription(Pattern pattern, TimeSpan waitBeforeWriteTimeout)
        {
            if (pattern is null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            this.Id = Guid.NewGuid();
            this.Pattern = pattern;
            this.waitBeforeWriteTimeout = waitBeforeWriteTimeout;
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

        public Task<bool> WaitUntilWrite(TimeSpan timeout) 
            => this.waitUntilWrite.WaitAsync(timeout);

        public void ContinueWrite() 
            => this.waitBeforeWrite.Set();

        public async Task WriteAsync(IEvent evt, CancellationToken cancellationToken = default)
        {
            await this.waitBeforeWrite.WaitAsync(this.waitBeforeWriteTimeout, cancellationToken);
            this.receivedEvents.Add(evt);
            this.waitUntilWrite.Set();
        }
    }
}
