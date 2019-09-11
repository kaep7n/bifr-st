using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Bifröst.Tests
{
    public partial class BusTests
    {
        [Fact]
        public void Ctor_should_create_instance()
        {
            var bus = new Bus();
            Assert.NotNull(bus);
        }

        [Fact]
        public void Start_then_Stop_should_set_IsRunning_accordingly()
        {
            var bus = new Bus();
            bus.Start();
            Assert.True(bus.IsRunning);
            bus.Stop();
            Assert.False(bus.IsRunning);
        }

        [Fact]
        public async Task Enqueue_should_take_event_withou_subscription()
        {
            var topic = new TopicBuilder("test").Build();

            var expectedEvent = new FakeEvent(topic, "Test");

            var bus = new Bus();
            await bus.EnqueueAsync(expectedEvent);
        }

        [Fact]
        public async Task Enqueue_should_forward_event_to_registered_subscriber()
        {
            using var resetEvent = new ManualResetEvent(false);
            
            var topic = new TopicBuilder("test").Build();
            var expectedEvent = new FakeEvent(topic, "Test");

            var subscription = new ActionSubscription(evt =>
            {
                Assert.Equal(expectedEvent, evt);
                resetEvent.Set();
            });

            var bus = new Bus();
            bus.Subscribe(subscription);
            await bus.EnqueueAsync(expectedEvent);
            bus.Start();

            var wasReset = resetEvent.WaitOne(50);
            Assert.True(wasReset);
        }
    }
}
