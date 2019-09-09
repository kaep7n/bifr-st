using System;

namespace Bifröst.Tests
{
    public partial class BusTests
    {
        public class FakeEvent : IEvent
        {
            public FakeEvent(Topic topic)
            {
                if (topic is null)
                {
                    throw new ArgumentNullException(nameof(topic));
                }

                this.Id = Guid.NewGuid();
                this.Topic = topic;
            }

            public Guid Id { get; }

            public Topic Topic { get; }
        }
    }
}
