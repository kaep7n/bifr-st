using Bifröst.Core;
using System;

namespace Bifröst.Benchmarking
{
    public class TestEvent : IEvent
    {
        public TestEvent(string topic, string message)
        {
            this.Id = Guid.NewGuid();
            this.Topic = topic;
            this.Message = message;
        }

        public Guid Id { get; }

        public string Topic { get; }

        public string Message { get; }
    }
}
