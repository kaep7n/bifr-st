using Bifröst.Subscriptions;
using Bifröst.Tests.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Bifröst.Tests
{
    public partial class ActionSubscriptionTests
    {

        [Fact]
        public void Ctor_should_create_instance_with_default_settings()
        {
            var bus = new FakeBus();
            var pattern = new PatternBuilder("root").Build();
            var subscription = new AsyncActionSubscription(bus, pattern, e => Task.CompletedTask);

            Assert.NotNull(subscription);
            Assert.NotEqual(Guid.Empty, subscription.Id);
            Assert.False(subscription.IsEnabled);
            Assert.Equal(pattern, subscription.Pattern);
        }

        [Fact]
        public void Enable_then_Disable_should_set_isEnabled_accordingly()
        {
            var bus = new FakeBus();
            var pattern = new PatternBuilder("root").Build();
            var subscription = new AsyncActionSubscription(bus, pattern, e => Task.CompletedTask);

            subscription.Enable();
            Thread.Sleep(10);
            Assert.True(subscription.IsEnabled);

            subscription.Disable();
            Thread.Sleep(10);
            Assert.False(subscription.IsEnabled);
        }

        [Theory]
        [InlineData(3)]
        public void Enable_then_Disable_multiple_times_should_set_isEnabled_accordingly(int times)
        {
            var bus = new FakeBus();
            var pattern = new PatternBuilder("root").Build();
            var subscription = new AsyncActionSubscription(bus, pattern, e => Task.CompletedTask);

            for (int i = 0; i < times; i++)
            {
                subscription.Enable();
                Thread.Sleep(10);
                Assert.True(subscription.IsEnabled);

                subscription.Disable();
                Thread.Sleep(10);
                Assert.False(subscription.IsEnabled);
            }
        }

        [Fact]
        public void Enable_then_Disable_should_subscribe_and_unsubscribe()
        {
            var bus = new FakeBus();
            var pattern = new PatternBuilder("root").Build();
            var subscription = new AsyncActionSubscription(bus, pattern, e => Task.CompletedTask);

            subscription.Enable();

            Assert.True(bus.IsSubscribed(subscription));

            subscription.Disable();

            Assert.False(bus.IsSubscribed(subscription));
        }

        [Fact]
        public async Task EnqueueAsync_when_not_enabled_should_not_call_registered_action()
        {
            using var resetEvent = new ManualResetEvent(false);

            var bus = new FakeBus();
            var topic = new TopicBuilder("root").Build();
            var pattern = new PatternBuilder("root").Build();

            var expectedEvent = new FakeEvent(topic, "Test");

            var subscription = new AsyncActionSubscription(bus, pattern, evt =>
            {
                Assert.Equal(expectedEvent, evt);
                resetEvent.Set();
                return Task.CompletedTask;
            });

            await subscription.EnqueueAsync(expectedEvent);

            var wasReset = resetEvent.WaitOne(50);
            Assert.False(wasReset);
        }

        [Theory]
        [ClassData(typeof(SingleEventData))]
        public async Task EnqueueAsync_after_Enabled_should_call_registered_action(IEvent evt)
        {
            using var resetEvent = new ManualResetEvent(false);

            var bus = new FakeBus();
            var pattern = new PatternBuilder().FromTopic(evt.Topic).Build();
            var subscription = new AsyncActionSubscription(bus, pattern, e =>
            {
                resetEvent.Set();
                return Task.CompletedTask;
            });
            
            subscription.Enable();

            await subscription.EnqueueAsync(evt);
            
            var wasReset = resetEvent.WaitOne(50);
            Assert.True(wasReset);
        }

        [Fact]
        public async Task EnqueueAsync_before_Enabled_should_call_registered_action_after_enabled()
        {
            using var resetEvent = new ManualResetEvent(false);

            var bus = new FakeBus();
            var topic = new TopicBuilder("root").Build();
            var pattern = new PatternBuilder("root").Build();
            var expectedEvent = new FakeEvent(topic, "Test");

            var subscription = new AsyncActionSubscription(bus, pattern, evt =>
            {
                Assert.Equal(expectedEvent, evt);
                resetEvent.Set();
                return Task.CompletedTask;
            });
            
            await subscription.EnqueueAsync(expectedEvent);

            subscription.Enable();

            var wasReset = resetEvent.WaitOne(50);
            Assert.True(wasReset);
        }
    }
}
