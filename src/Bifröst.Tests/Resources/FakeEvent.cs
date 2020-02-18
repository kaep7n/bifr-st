using System;

namespace Bifröst.Tests.Resources
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
