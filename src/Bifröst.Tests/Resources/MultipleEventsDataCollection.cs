using System.Collections.Generic;
using Xunit;

namespace Bifröst.Tests.Resources
{
    public class MultipleEventsDataCollection : TheoryData<Topic, IEnumerable<IEvent>>
    {
        public MultipleEventsDataCollection()
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
