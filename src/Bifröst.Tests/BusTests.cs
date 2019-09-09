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
        public void Subscribe_should_start_bus_when_than_one_subscription_added()
        {
            var subscription = new ActionSubscription(evt => { });
            
            var bus = new Bus();
            bus.Subscribe(subscription);
            Assert.True(bus.IsStarted);
        }

        [Fact]
        public void Unsubscribe_should_stop_bus_when_no_subscribers_left()
        {
            var subscription = new ActionSubscription(evt => { });
            
            var bus = new Bus();
            bus.Subscribe(subscription);
            Assert.True(bus.IsStarted);
            bus.Unsubscribe(subscription);
            Assert.False(bus.IsStarted);
        }
    }
}
