using System;

namespace Bifröst.Playground
{
    public class DataEvent : IEvent
    {
        public DataEvent(Topic topic, Data data)
        {
            if (topic is null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.Id = Guid.NewGuid();
            this.Topic = topic;
            this.Data = data;
        }

        public Guid Id { get; }

        public Topic Topic { get; }

        public Data Data { get; }
    }
}
