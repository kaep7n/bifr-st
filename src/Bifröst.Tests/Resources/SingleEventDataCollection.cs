using Xunit;

namespace Bifröst.Tests.Resources
{
    public class SingleEventDataCollection : TheoryData<IEvent>
    {
        public SingleEventDataCollection()
        {
            var topic = new TopicBuilder("test").Build();
            this.Add(new FakeEvent(topic, "Test"));
        }
    }
}
