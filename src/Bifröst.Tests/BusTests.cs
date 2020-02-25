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
        private readonly TimeSpan waitTimeout = TimeSpan.FromMilliseconds(200);

        [Fact]
        public void Ctor_should_create_instance_with_default_settings()
        {
            using var bus = new Bus();
            Assert.NotNull(bus);
            Assert.False(bus.IsRunning);
        }

        [Fact]
        public async Task RunAsync_then_Idle_should_set_IsRunning_accordingly()
        {
            using var bus = new Bus();
            await bus.RunAsync().ConfigureAwait(false);
            Assert.True(bus.IsRunning);

            await bus.IdleAsync().ConfigureAwait(false);
            Assert.False(bus.IsRunning);
        }

        [Theory]
        [InlineData(3)]
        public async Task Run_then_Idle_multiple_times_should_set_IsRunning_accordingly(int times)
        {
            using var bus = new Bus();

            for (var i = 0; i < times; i++)
            {
                await bus.RunAsync().ConfigureAwait(false);
                Assert.True(bus.IsRunning);

                await bus.IdleAsync().ConfigureAwait(false);
                Assert.False(bus.IsRunning);
            }
        }

        [Fact]
        public async Task WriteAsync_should_take_event_without_subscription()
        {
            var topic = new TopicBuilder("root").Build();

            var expectedEvent = new FakeEvent(topic, "Test");

            using var bus = new Bus();

            await bus.WriteAsync(expectedEvent).ConfigureAwait(false);
        }

        [Theory]
        [ClassData(typeof(SingleEventDataCollection))]
        public async Task WriteAsync_should_forward_event_to_registered_subscriber(IEvent evt)
        {
            if (evt is null)
                throw new ArgumentNullException(nameof(evt));

            var pattern = new PatternBuilder().FromTopic(evt.Topic).Build();
            var subscription = new FakeSubscription(pattern, this.waitTimeout);
            await subscription.EnableAsync()
                .ConfigureAwait(false);

            using var bus = new Bus();

            await bus.RunAsync()
                .ConfigureAwait(false);

            bus.Subscribe(subscription);

            await bus.WriteAsync(evt)
                .ConfigureAwait(false);

            subscription.ContinueWrite();

            await subscription.WaitUntilWrite(this.waitTimeout)
                .ConfigureAwait(false);

            Assert.Collection(subscription.ReceivedEvents, e => Assert.Equal(evt, e));
        }

        [Theory]
        [ClassData(typeof(SingleEventDataCollection))]
        public async Task WriteAsync_should_set_metric_received_events_to_one(IEvent evt)
        {
            using var bus = new Bus();

            await bus.WriteAsync(evt).ConfigureAwait(false);

            var metrics = bus.GetMetrics();

            var metric = Assert.Single(metrics, m => m.Name == Metrics.Bus.ReceivedEvents);
            Assert.Equal(1L, metric.Value);
        }

        [Theory]
        [ClassData(typeof(MultipleEventsDataCollection))]
        public async Task WriteAsync_should_set_metric_received_events_to_input_event_count(Topic topic, IEnumerable<IEvent> events)
        {
            if (topic is null)
                throw new ArgumentNullException(nameof(topic));
            if (events is null)
                throw new ArgumentNullException(nameof(events));

            using var bus = new Bus();

            foreach (var evt in events)
            {
                await bus.WriteAsync(evt).ConfigureAwait(false);
            }

            var metrics = bus.GetMetrics();

            var metric = Assert.Single(metrics, m => m.Name == Metrics.Bus.ReceivedEvents);
            Assert.Equal(events.LongCount(), metric.Value);
        }

        [Theory]
        [ClassData(typeof(SingleEventDataCollection))]
        public async Task WriteAsync_should_increment_processed_event_count_to_one(IEvent evt)
        {
            if (evt is null)
                throw new ArgumentNullException(nameof(evt));

            var pattern = new PatternBuilder().FromTopic(evt.Topic).Build();
            var subscription = new FakeSubscription(pattern, this.waitTimeout);
            await subscription.EnableAsync().ConfigureAwait(false);

            using var bus = new Bus();

            await bus.RunAsync().ConfigureAwait(false);
            bus.Subscribe(subscription);

            await bus.WriteAsync(evt).ConfigureAwait(false);
            subscription.ContinueWrite();
            await subscription.WaitUntilWrite(this.waitTimeout).ConfigureAwait(false);

            var metrics = bus.GetMetrics();

            var metric = Assert.Single(metrics, m => m.Name == Metrics.Bus.ProcessedEvents);
            Assert.Equal(1L, metric.Value);
        }

        [Theory]
        [ClassData(typeof(MultipleEventsDataCollection))]
        public async Task WriteAsync_should_increment_processed_event_count_to_input_event_count(Topic topic, IEnumerable<IEvent> events)
        {
            if (topic is null)
                throw new ArgumentNullException(nameof(topic));
            if (events is null)
                throw new ArgumentNullException(nameof(events));

            var pattern = new PatternBuilder().FromTopic(topic).Build();
            var subscription = new FakeSubscription(pattern, this.waitTimeout);

            using var bus = new Bus();

            await bus.RunAsync().ConfigureAwait(false);
            bus.Subscribe(subscription);

            foreach (var evt in events)
            {
                await bus.WriteAsync(evt).ConfigureAwait(false);
                subscription.ContinueWrite();
                await subscription.WaitUntilWrite(this.waitTimeout).ConfigureAwait(false);
            }

            var metrics = bus.GetMetrics();

            var metric = Assert.Single(metrics, m => m.Name == Metrics.Bus.ProcessedEvents);
            Assert.Equal(events.LongCount(), metric.Value);
        }

        [Theory]
        [ClassData(typeof(MultipleEventsDataCollection))]
        public async Task WriteAsync_should_forward_multiple_events_to_registered_subscriber(Topic topic, IEnumerable<IEvent> events)
        {
            if (topic is null)
                throw new ArgumentNullException(nameof(topic));
            if (events is null)
                throw new ArgumentNullException(nameof(events));

            var pattern = new PatternBuilder().FromTopic(topic).Build();
            var subscription = new FakeSubscription(pattern, this.waitTimeout);

            using var bus = new Bus();

            await bus.RunAsync().ConfigureAwait(false);
            bus.Subscribe(subscription);

            subscription.ContinueWrite();

            foreach (var evt in events)
            {
                await bus.WriteAsync(evt).ConfigureAwait(false);
                subscription.ContinueWrite();
                await subscription.WaitUntilWrite(this.waitTimeout).ConfigureAwait(false);
            }

            Assert.All(events, e => Assert.Contains(subscription.ReceivedEvents, r => r.Id == e.Id));
        }

        [Theory]
        [ClassData(typeof(SingleEventDataCollection))]
        public async Task WriteAsync_should_forward_event_to_subscribers_if_first_subscriber_is_blocking(IEvent evt)
        {
            if (evt is null)
                throw new ArgumentNullException(nameof(evt));

            var pattern = new PatternBuilder().FromTopic(evt.Topic).Build();

            var blockingSubscription = new FakeSubscription(pattern, Timeout.InfiniteTimeSpan);
            await blockingSubscription.EnableAsync().ConfigureAwait(false);

            var subscription = new FakeSubscription(pattern, this.waitTimeout);
            await subscription.EnableAsync().ConfigureAwait(false);

            using var bus = new Bus();

            await bus.RunAsync().ConfigureAwait(false);

            bus.Subscribe(blockingSubscription);
            bus.Subscribe(subscription);

            await bus.WriteAsync(evt).ConfigureAwait(false);
            subscription.ContinueWrite();

            await subscription.WaitUntilWrite(this.waitTimeout).ConfigureAwait(false);

            Assert.Single(subscription.ReceivedEvents, evt);
            Assert.Empty(blockingSubscription.ReceivedEvents);
        }

        [Theory]
        [ClassData(typeof(MultipleEventsDataCollection))]
        public async Task WriteAsync_should_forward_multiple_events_to_subscribers_if_first_subscriber_is_blocking(Topic topic, IEnumerable<IEvent> events)
        {
            if (topic is null)
                throw new ArgumentNullException(nameof(topic));
            if (events is null)
                throw new ArgumentNullException(nameof(events));

            var pattern = new PatternBuilder().FromTopic(topic).Build();

            var blockingSubscription = new FakeSubscription(pattern, Timeout.InfiniteTimeSpan);
            await blockingSubscription.EnableAsync().ConfigureAwait(false);

            var subscription = new FakeSubscription(pattern, this.waitTimeout);
            await subscription.EnableAsync().ConfigureAwait(false);

            using var bus = new Bus();

            await bus.RunAsync().ConfigureAwait(false);

            bus.Subscribe(blockingSubscription);
            bus.Subscribe(subscription);

            foreach (var evt in events)
            {
                await bus.WriteAsync(evt).ConfigureAwait(false);
                subscription.ContinueWrite();
                await subscription.WaitUntilWrite(this.waitTimeout).ConfigureAwait(false);
            }

            Assert.All(events, e => Assert.Contains(subscription.ReceivedEvents, r => r.Id == e.Id));
            Assert.Empty(blockingSubscription.ReceivedEvents);
        }
    }
}
