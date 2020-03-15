using Bifröst.Subscriptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bifröst.Tests.Resources
{
    public class FakeBus : IBus
    {
        private readonly List<IEvent> enqueuedEvents = new List<IEvent>();
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();

        public bool IsRunning { get; private set; }

        public Task WriteAsync(IEvent evt)
        {
            this.enqueuedEvents.Add(evt);
            return Task.CompletedTask;
        }

        public Task IdleAsync(CancellationToken cancellationToken = default)
        {
            this.IsRunning = false;
            return Task.CompletedTask;
        }

        public Task RunAsync(CancellationToken cancellationToken = default)
        {
            this.IsRunning = true;
            return Task.CompletedTask;
        }

        public void Subscribe(ISubscription subscription) => this.subscriptions.Add(subscription);

        public void Unsubscribe(ISubscription subscription) => this.subscriptions.Remove(subscription);

        public bool IsSubscribed(ISubscription subscription) => this.subscriptions.Contains(subscription);
    }
}
