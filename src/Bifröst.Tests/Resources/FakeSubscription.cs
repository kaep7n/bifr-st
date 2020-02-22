using Bifröst.Subscriptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bifröst.Tests.Resources
{
    public class FakeSubscription : ISubscription
    {
        private readonly AsyncAutoResetEvent waitBeforeWrite = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent waitUntilWrite = new AsyncAutoResetEvent(false);
        private readonly List<IEvent> receivedEvents = new List<IEvent>();

        public FakeSubscription(Pattern pattern)
        {
            if (pattern is null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            this.Id = Guid.NewGuid();
            this.Pattern = pattern;
        }

        public Guid Id { get; }

        public bool IsEnabled { get; private set; }

        public Pattern Pattern { get; }

        public IEnumerable<IEvent> ReceivedEvents => this.receivedEvents;

        public void Disable()
        {
            this.IsEnabled = false;
        }

        public void Enable()
        {
            this.IsEnabled = true;
        }
        
        public bool Matches(Topic topic)
        {
            return this.Pattern.Matches(topic);
        }

        public Task<bool> WaitUntilWrite(TimeSpan timeout) 
            => this.waitUntilWrite.WaitAsync(timeout);

        public void ContinueWrite() 
            => this.waitBeforeWrite.Set();

        public async Task WriteAsync(IEvent evt)
        {
            await this.waitBeforeWrite.WaitAsync(TimeSpan.FromSeconds(1));
            this.receivedEvents.Add(evt);
            this.waitUntilWrite.Set();
        }
    }
}
