using System;

namespace Bifröst.Playground.Events
{
    public class ValueEvent : IEvent
    {
        public ValueEvent(Topic topic, int value)
        {
            if (topic is null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            this.Id = Guid.NewGuid();
            this.Topic = topic;
            this.Value = value;
        }

        public Guid Id { get; }

        public Topic Topic { get; }

        public int Value { get; }
    }
}
