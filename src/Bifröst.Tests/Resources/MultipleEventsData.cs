using System.Collections.Generic;
using Xunit;

namespace Bifröst.Tests.Resources
{
    public class MultipleEventsData : TheoryData<Topic, IEnumerable<IEvent>>
    {
        public MultipleEventsData()
        {
            var topic = new TopicBuilder("test").Build();

            this.AddRow(topic, new[] {
                new FakeEvent(topic, "Test"),
                new FakeEvent(topic, "Test"),
                new FakeEvent(topic, "Test"),
                new FakeEvent(topic, "Test"),
                new FakeEvent(topic, "Test")
            });
        }
    }
}
