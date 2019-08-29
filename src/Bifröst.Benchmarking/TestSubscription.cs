using Bifröst.Core;
using System;

namespace Bifröst.Benchmarking
{
    public class TestSubscription : ISubscription
    {
        public TestSubscription(string topic)
        {
            this.Id = Guid.NewGuid();
            this.Topic = topic;
        }

        public Guid Id { get; }

        public string Topic { get; }

        public void Receive(IEvent evt)
        {
        }
    }
}
