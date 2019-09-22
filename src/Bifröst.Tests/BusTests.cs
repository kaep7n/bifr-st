using System.Collections.Generic;
using System.Linq;
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
        [InlineData(3)]
        public void Start_then_Stop_multiple_times_should_set_IsRunning_accordingly(int times)
        {
            var bus = new Bus();

            for (var i = 0; i < times; i++)
            {
                bus.Start();
                Thread.Sleep(5);
                Assert.True(bus.IsRunning);

                bus.Stop();
                Thread.Sleep(5);
                Assert.False(bus.IsRunning);
            }
        }

        [Fact]
        public async Task EnqueueAsync_should_take_event_without_subscription()
        {
            var topic = new TopicBuilder("root").Build();

            var expectedEvent = new FakeEvent(topic, "Test");

            var bus = new Bus();
            await bus.EnqueueAsync(expectedEvent);
        }

        [Theory]
        [ClassData(typeof(SingleEventData))]
        public async Task EnqueueAsync_should_forward_event_to_registered_subscriber(IEvent evt)
        {
            var subscription = new FakeSubscription((Topic)evt.Topic.Clone());

            var bus = new Bus();
            bus.Start();
            bus.Subscribe(subscription);

            await bus.EnqueueAsync(evt);

            Assert.Collection(subscription.ReceivedEvents, e => Assert.Equal(evt, e));
        }

        [Theory]
        [ClassData(typeof(MultipleEventsData))]
        public async Task EnqueueAsync_should_forward_multiple_events_to_registered_subscriber(Topic topic, IEnumerable<IEvent> events)
        {
            var subscription = new FakeSubscription(topic);

            var bus = new Bus();
            bus.Start();
            bus.Subscribe(subscription);

            foreach (var evt in events)
            {
                await bus.EnqueueAsync(evt);
            }

            Assert.All(events, e => Assert.Contains(subscription.ReceivedEvents, r => r.Id == e.Id));
        }
    }
}
