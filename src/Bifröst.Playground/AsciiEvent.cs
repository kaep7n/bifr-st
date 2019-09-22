using System;

namespace Bifröst.Playground
{
    public class AsciiEvent : IEvent
    {
        public AsciiEvent(Topic topic, string key, char ascii)
        {
            if (topic is null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            this.Id = Guid.NewGuid();
            this.Topic = topic;
            this.Key = key;
            this.Ascii = ascii;
        }

        public Guid Id { get; }

        public Topic Topic { get; }

        public string Key { get; }
        
        public char Ascii { get; }
    }
}
