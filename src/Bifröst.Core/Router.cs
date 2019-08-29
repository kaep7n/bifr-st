using System.Collections.Generic;
using System.Linq;

namespace Bifröst.Core
{
    public class Router
    {
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();

        public void Subscribe(ISubscription subscription)
        {
            this.subscriptions.Add(subscription);
        }

        public void Enqueue(IEvent evt)
        {
            var receivers = this.subscriptions.Where(s => s.Topic == evt.Topic);

            foreach (var receiver in receivers)
            {
                receiver.Receive(evt);
            }
        }
    }
}
