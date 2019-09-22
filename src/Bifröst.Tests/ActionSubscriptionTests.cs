using System;
using System.Collections.Generic;
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
            var topic = new TopicBuilder("root").Build();
            var subscription = new AsyncActionSubscription(e => Task.CompletedTask, topic);

            Assert.NotNull(subscription);
            Assert.NotEqual(Guid.Empty, subscription.Id);
            Assert.False(subscription.IsEnabled);
            Assert.Collection(subscription.Topics, t => t.Equals(topic));
        }

        [Fact]
        public void Enable_then_Disable_should_set_isEnabled_accordingly()
        {
            var topic = new TopicBuilder("root").Build();
            var subscription = new AsyncActionSubscription(e => Task.CompletedTask, topic);

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
            var topic = new TopicBuilder("root").Build();
            var subscription = new AsyncActionSubscription(e => Task.CompletedTask, topic);

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
        public async Task EnqueueAsync_when_not_enabled_should_not_call_registered_action()
        {
            using var resetEvent = new ManualResetEvent(false);

            var topic = new TopicBuilder("test").Build();
            var expectedEvent = new FakeEvent(topic, "Test");

            var subscription = new AsyncActionSubscription(evt =>
            {
                Assert.Equal(expectedEvent, evt);
                resetEvent.Set();
                return Task.CompletedTask;
            }, (Topic)topic.Clone());

            await subscription.EnqueueAsync(expectedEvent);

            var wasReset = resetEvent.WaitOne(50);
            Assert.False(wasReset);
        }

        [Theory]
        [ClassData(typeof(SingleEventData))]
        public async Task EnqueueAsync_after_Enabled_should_call_registered_action(IEvent evt)
        {
            using var resetEvent = new ManualResetEvent(false);

            var subscription = new AsyncActionSubscription(e =>
            {
                resetEvent.Set();
                return Task.CompletedTask;
            }, (Topic)evt.Topic.Clone());
            
            subscription.Enable();

            await subscription.EnqueueAsync(evt);
            
            var wasReset = resetEvent.WaitOne(50);
            Assert.True(wasReset);
        }

        [Fact]
        public async Task EnqueueAsync_before_Enabled_should_call_registered_action_after_enabled()
        {
            using var resetEvent = new ManualResetEvent(false);

            var topic = new TopicBuilder("test").Build();
            var expectedEvent = new FakeEvent(topic, "Test");

            var subscription = new AsyncActionSubscription(evt =>
            {
                Assert.Equal(expectedEvent, evt);
                resetEvent.Set();
                return Task.CompletedTask;
            }, (Topic)topic.Clone());
            
            await subscription.EnqueueAsync(expectedEvent);

            subscription.Enable();

            var wasReset = resetEvent.WaitOne(50);
            Assert.True(wasReset);
        }
    }
}
