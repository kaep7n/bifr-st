using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bifröst.Tests
{
    public class FakeSubscription : ISubscription
    {
        private readonly List<IEvent> receivedEvents = new List<IEvent>();

        public FakeSubscription(params Topic[] topics)
        {
            if (topics is null)
            {
                throw new ArgumentNullException(nameof(topics));
            }

            this.Id = Guid.NewGuid();
            this.Topics = topics;
        }

        public Guid Id { get; }

        public bool IsEnabled { get; private set; }

        public IEnumerable<Topic> Topics { get; }

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
            return this.Topics.Any(t => t.Equals(topic));
        }

        public Task EnqueueAsync(IEvent evt)
        {
            this.receivedEvents.Add(evt);
            return Task.CompletedTask;
        }
    }
}
