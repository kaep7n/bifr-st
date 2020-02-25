using Bifröst.Subscriptions;
using Bifröst.Tests.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Bifröst.Tests
{
    public class ActionSubscriptionTests
    {
        private readonly TimeSpan waitTimeout = TimeSpan.FromMilliseconds(1000);

        [Fact]
        public void Ctor_should_create_instance_with_default_settings()
        {
            var bus = new FakeBus();
            var pattern = new PatternBuilder("root").Build();
            using var subscription = new AsyncActionSubscription(bus, pattern, e => Task.CompletedTask);

            Assert.NotNull(subscription);
            Assert.NotEqual(Guid.Empty, subscription.Id);
            Assert.False(subscription.IsEnabled);
            Assert.Equal(pattern, subscription.Pattern);
        }

        [Fact]
        public async Task Enable_then_Disable_should_set_isEnabled_accordingly()
        {
            var bus = new FakeBus();
            var pattern = new PatternBuilder("root").Build();
            using var subscription = new AsyncActionSubscription(bus, pattern, e => Task.CompletedTask);

            await subscription.EnableAsync().ConfigureAwait(false);
            Assert.True(subscription.IsEnabled);

            await subscription.DisableAsync().ConfigureAwait(false);
            Assert.False(subscription.IsEnabled);
        }

        [Theory]
        [InlineData(3)]
        public async Task Enable_then_Disable_multiple_times_should_set_isEnabled_accordingly(int times)
        {
            var bus = new FakeBus();
            var pattern = new PatternBuilder("root").Build();
            using var subscription = new AsyncActionSubscription(bus, pattern, e => Task.CompletedTask);

            for (int i = 0; i < times; i++)
            {
                await subscription.EnableAsync().ConfigureAwait(false);
                Assert.True(subscription.IsEnabled);

                await subscription.DisableAsync().ConfigureAwait(false);
                Assert.False(subscription.IsEnabled);
            }
        }

        [Fact]
        public async Task Enable_then_Disable_should_subscribe_and_unsubscribe()
        {
            var bus = new FakeBus();
            var pattern = new PatternBuilder("root").Build();
            using var subscription = new AsyncActionSubscription(bus, pattern, e => Task.CompletedTask);

            await subscription.EnableAsync().ConfigureAwait(false);

            Assert.True(bus.IsSubscribed(subscription));

            await subscription.DisableAsync().ConfigureAwait(false);

            Assert.False(bus.IsSubscribed(subscription));
        }

        [Fact]
        public async Task WriteAsync_when_not_enabled_should_not_call_registered_action()
        {
            using var resetEvent = new ManualResetEvent(false);

            var bus = new FakeBus();
            var topic = new TopicBuilder("root").Build();
            var pattern = new PatternBuilder("root").Build();

            var expectedEvent = new FakeEvent(topic, "Test");

            using var subscription = new AsyncActionSubscription(bus, pattern, evt =>
            {
                Assert.Equal(expectedEvent, evt);
                resetEvent.Set();
                return Task.CompletedTask;
            });

            await subscription.WriteAsync(expectedEvent).ConfigureAwait(false);

            var wasReset = resetEvent.WaitOne(this.waitTimeout);
            Assert.False(wasReset);
        }

        [Theory]
        [ClassData(typeof(SingleEventDataCollection))]
        public async Task WriteAsync_after_Enabled_should_call_registered_action(IEvent evt)
        {
            if (evt is null)
                throw new ArgumentNullException(nameof(evt));

            using var resetEvent = new ManualResetEvent(false);

            var bus = new FakeBus();
            var pattern = new PatternBuilder().FromTopic(evt.Topic).Build();
            using var subscription = new AsyncActionSubscription(bus, pattern, e =>
            {
                resetEvent.Set();
                return Task.CompletedTask;
            });

            await subscription.EnableAsync().ConfigureAwait(false);

            await subscription.WriteAsync(evt).ConfigureAwait(false);

            var wasReset = resetEvent.WaitOne(this.waitTimeout);
            Assert.True(wasReset);
        }

        [Theory]
        [ClassData(typeof(SingleEventDataCollection))]
        public async Task WriteAsync_should_set_metric_received_events_to_one(IEvent evt)
        {
            if (evt is null)
                throw new ArgumentNullException(nameof(evt));

            var bus = new FakeBus();
            var pattern = new PatternBuilder().FromTopic(evt.Topic).Build();
            using var subscription = new AsyncActionSubscription(bus, pattern, e => Task.CompletedTask);

            await subscription.EnableAsync().ConfigureAwait(false);

            await subscription.WriteAsync(evt).ConfigureAwait(false);

            var metric = Assert.Single(subscription.GetMetrics(), m => m.Name == Metrics.Subscription.ReceivedEvents);
            Assert.Equal(1L, metric.Value);
        }

        [Theory]
        [ClassData(typeof(SingleEventDataCollection))]
        public async Task WriteAsync_should_set_metric_processed_events_to_one(IEvent evt)
        {
            if (evt is null)
                throw new ArgumentNullException(nameof(evt));

            using var resetEvent = new ManualResetEvent(false);

            var bus = new FakeBus();
            var pattern = new PatternBuilder().FromTopic(evt.Topic).Build();
            using var subscription = new AsyncActionSubscription(bus, pattern, e =>
            {
                resetEvent.Set();
                return Task.CompletedTask;
            });

            await subscription.EnableAsync().ConfigureAwait(false);

            await subscription.WriteAsync(evt).ConfigureAwait(false);

            var wasReset = resetEvent.WaitOne(this.waitTimeout);
            Assert.True(wasReset);

            var metric = Assert.Single(subscription.GetMetrics(), m => m.Name == Metrics.Subscription.ProcessedEvents);
            Assert.Equal(1L, metric.Value);
        }

        [Fact]
        public async Task WriteAsync_before_Enabled_should_call_registered_action_after_enabled()
        {
            using var resetEvent = new ManualResetEvent(false);

            var bus = new FakeBus();
            var topic = new TopicBuilder("root").Build();
            var pattern = new PatternBuilder("root").Build();
            var expectedEvent = new FakeEvent(topic, "Test");

            using var subscription = new AsyncActionSubscription(bus, pattern, evt =>
            {
                Assert.Equal(expectedEvent, evt);
                resetEvent.Set();
                return Task.CompletedTask;
            });

            await subscription.WriteAsync(expectedEvent).ConfigureAwait(false);

            await subscription.EnableAsync().ConfigureAwait(false);

            var wasReset = resetEvent.WaitOne(this.waitTimeout);
            Assert.True(wasReset);
        }
    }
}
