using System;

namespace Bifröst.Tests
{
    public class FakeEvent : IEvent
    {
        public FakeEvent(Topic topic, string message)
        {
            this.Id = Guid.NewGuid();
            this.Topic = topic;
            this.Message = message;
        }

        public Guid Id { get; }

        public Topic Topic { get; }

        public string Message { get; }
    }
}
