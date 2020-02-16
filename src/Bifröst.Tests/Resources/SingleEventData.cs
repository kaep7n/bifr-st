using Xunit;

namespace Bifröst.Tests.Resources
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
