using Xunit;

namespace Bifröst.Tests
{
    public class SingleEventData : TheoryData<IEvent>
    {
        public SingleEventData()
        {
            var topic = new TopicBuilder("test").Build();
            this.Add(new FakeEvent(topic, "Test"));
        }
    }
}
