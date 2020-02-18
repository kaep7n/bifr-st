using Bifröst.Tests.Resources;
using System;
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
            using var bus = new Bus();
            Assert.NotNull(bus);
            Assert.False(bus.IsRunning);
        }

        [Fact]
        public void Run_then_Idle_should_set_IsRunning_accordingly()
        {
            using var bus = new Bus();
            bus.Run();

            Thread.Sleep(10);
            Assert.True(bus.IsRunning);

            bus.Idle();
            Thread.Sleep(10);
            Assert.False(bus.IsRunning);
        }

        [Theory]
        [InlineData(3)]
        public void Run_then_Idle_multiple_times_should_set_IsRunning_accordingly(int times)
        {
            using var bus = new Bus();

            for (var i = 0; i < times; i++)
            {
                bus.Run();
                Thread.Sleep(10);
                Assert.True(bus.IsRunning);

                bus.Idle();
                Thread.Sleep(10);
                Assert.False(bus.IsRunning);
            }
        }

        [Fact]
        public async Task WriteAsync_should_take_event_without_subscription()
        {
            var topic = new TopicBuilder("root").Build();

            var expectedEvent = new FakeEvent(topic, "Test");

            using var bus = new Bus();
            
            await bus.WriteAsync(expectedEvent);
        }

        [Theory]
        [ClassData(typeof(SingleEventData))]
        public async Task WriteAsync_should_forward_event_to_registered_subscriber(IEvent evt)
        {
            var pattern = new PatternBuilder().FromTopic(evt.Topic).Build();
            var subscription = new FakeSubscription(pattern);
            subscription.Enable();

            using var bus = new Bus();

            bus.Run();
            bus.Subscribe(subscription);

            await bus.WriteAsync(evt);
            subscription.ContinueWrite();
            await subscription.WaitUntilWrite(TimeSpan.FromMilliseconds(100));

            Assert.Collection(subscription.ReceivedEvents, e => Assert.Equal(evt, e));
        }

        [Theory]
        [ClassData(typeof(SingleEventData))]
        public async Task WriteAsync_should_set_metric_received_events_to_one(IEvent evt)
        {
            using var bus = new Bus();
            
            await bus.WriteAsync(evt);

            var metrics = bus.GetMetrics();

            var metric = Assert.Single(metrics, m => m.Name == Metrics.BUS_RECEIVED_EVENTS);
            Assert.Equal(1L, metric.Value);
        }

        [Theory]
        [ClassData(typeof(MultipleEventsData))]
        public async Task WriteAsync_should_set_metric_received_events_to_input_event_count(Topic topic, IEnumerable<IEvent> events)
        {
            using var bus = new Bus();

            foreach (var evt in events)
            {
                await bus.WriteAsync(evt);
            }
            
            var metrics = bus.GetMetrics();

            var metric = Assert.Single(metrics, m => m.Name == Metrics.BUS_RECEIVED_EVENTS);
            Assert.Equal(events.LongCount(), metric.Value);
        }

        [Theory]
        [ClassData(typeof(SingleEventData))]
        public async Task WriteAsync_should_increment_processed_event_count_to_one(IEvent evt)
        {
            var pattern = new PatternBuilder().FromTopic(evt.Topic).Build();
            var subscription = new FakeSubscription(pattern);
            subscription.Enable();

            using var bus = new Bus();

            bus.Run();
            bus.Subscribe(subscription);

            await bus.WriteAsync(evt);
            subscription.ContinueWrite();
            await subscription.WaitUntilWrite(TimeSpan.FromMilliseconds(100));

            var metrics = bus.GetMetrics();

            var metric = Assert.Single(metrics, m => m.Name == Metrics.BUS_PROCESSED_EVENTS);
            Assert.Equal(1L, metric.Value);
        }

        [Theory]
        [ClassData(typeof(MultipleEventsData))]
        public async Task WriteAsync_should_increment_processed_event_count_to_input_event_count(Topic topic, IEnumerable<IEvent> events)
        {
            var pattern = new PatternBuilder().FromTopic(topic).Build();
            var subscription = new FakeSubscription(pattern);

            using var bus = new Bus();

            bus.Run();
            bus.Subscribe(subscription);

            foreach (var evt in events)
            {
                await bus.WriteAsync(evt);
                subscription.ContinueWrite();
                await subscription.WaitUntilWrite(TimeSpan.FromMilliseconds(100));
            }

            var metrics = bus.GetMetrics();

            var metric = Assert.Single(metrics, m => m.Name == Metrics.BUS_PROCESSED_EVENTS);
            Assert.Equal(events.LongCount(), metric.Value);
        }

        [Theory]
        [ClassData(typeof(MultipleEventsData))]
        public async Task WriteAsync_should_forward_multiple_events_to_registered_subscriber(Topic topic, IEnumerable<IEvent> events)
        {
            var pattern = new PatternBuilder().FromTopic(topic).Build();
            var subscription = new FakeSubscription(pattern);

            using var bus = new Bus();
           
            bus.Run();
            bus.Subscribe(subscription);
            
            subscription.ContinueWrite();

            foreach (var evt in events)
            {
                await bus.WriteAsync(evt);
                subscription.ContinueWrite();
                await subscription.WaitUntilWrite(TimeSpan.FromMilliseconds(100));
            }

            Assert.All(events, e => Assert.Contains(subscription.ReceivedEvents, r => r.Id == e.Id));
        } 
    }
}
