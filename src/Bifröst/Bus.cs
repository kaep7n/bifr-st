using Bifröst.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bifröst
{
    public sealed class Bus
    {
        private readonly Queue<IEvent> eventsQueue = new Queue<IEvent>();
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();

        public bool IsStarted { get; private set; } = false;

        public void Subscribe(ISubscription subscription)
        {
            this.subscriptions.Add(subscription);

            if (this.subscriptions.Any())
            {
                this.Start();
            }
        }

        public void Unsubscribe(ISubscription subscription)
        {
            this.subscriptions.Remove(subscription);

            if (!this.subscriptions.Any())
            {
                this.Stop();
            }
        }

        public void Enqueue(IEvent evt)
        {
            this.eventsQueue.Enqueue(evt);
        }

        private void Start()
        {
            this.IsStarted = true;
            Task.Run(() => this.Process());
        }

        private void Stop()
        {
            this.IsStarted = false;
        }

        private void Process()
        {
            while (this.IsStarted)
            {
                var evt = this.eventsQueue.Peek();

                if(evt == null)
                {
                    Thread.Sleep(1);
                }

                this.subscriptions
                    .Where(s => s.Matches(evt.Topic))
                    .ForEach(s => s.Receive(evt));
            }
        }
    }
}
