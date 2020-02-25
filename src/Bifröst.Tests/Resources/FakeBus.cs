using Bifröst.Subscriptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bifröst.Tests.Resources
{
    public class FakeBus : IBus
    {
        private List<IEvent> enqueuedEvents = new List<IEvent>();
        private List<ISubscription> subscriptions = new List<ISubscription>();

        public bool IsRunning { get; private set; }

        public Task WriteAsync(IEvent evt)
        {
            this.enqueuedEvents.Add(evt);
            return Task.CompletedTask;
        }

        public Task IdleAsync()
        {
            this.IsRunning = false;
            return Task.CompletedTask;
        }

        public Task RunAsync()
        {
            this.IsRunning = true;
            return Task.CompletedTask;
        }

        public void Subscribe(ISubscription subscription)
        {
            this.subscriptions.Add(subscription);
        }

        public void Unsubscribe(ISubscription subscription)
        {
            this.subscriptions.Remove(subscription);
        }

        public bool IsSubscribed(ISubscription subscription)
        {
            return this.subscriptions.Contains(subscription);
        }
    }
}
