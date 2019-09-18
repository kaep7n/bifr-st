using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Bifröst.Tests
{
    public partial class BusTests
    {
        [Fact]
        public void Ctor_should_create_instance_with_default_settings()
        {
            var bus = new Bus();
            Assert.NotNull(bus);
            Assert.False(bus.IsRunning);
        }

        [Fact]
        public void Start_then_Stop_should_set_IsRunning_accordingly()
        {
            var bus = new Bus();
            bus.Start();

            Thread.Sleep(10);
            Assert.True(bus.IsRunning);

            bus.Stop();
            Thread.Sleep(10);
            Assert.False(bus.IsRunning);
        }

        [Theory]
        [InlineData(5)]
        public void Start_then_Stop_multiple_times_should_set_IsRunning_accordingly(int times)
        {
            var bus = new Bus();
            
            for (var i = 0; i < times; i++)
            {
                bus.Start();
                Thread.Sleep(100);
                Assert.True(bus.IsRunning);
                bus.Stop();
                Thread.Sleep(100);
                Assert.False(bus.IsRunning);
            }
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

            var subscription = new AsyncActionSubscription(evt =>
            {
                Assert.Equal(expectedEvent, evt);
                resetEvent.Set();
                return Task.CompletedTask;
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
