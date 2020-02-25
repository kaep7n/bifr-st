using System;

namespace Bifröst.Playground.Events
{
    public class AsciiEvent : IEvent
    {
        public AsciiEvent(Topic topic, char ascii)
        {
            if (topic is null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            this.Id = Guid.NewGuid();
            this.Topic = topic;
            this.Ascii = ascii;
        }

        public Guid Id { get; }

        public Topic Topic { get; }

        public char Ascii { get; }
    }
}
