using System;

namespace Bifröst
{
    public class ActionSubscription : ISubscription
    {
        private readonly Action<IEvent> action;

        public ActionSubscription(Action<IEvent> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.Id = Guid.NewGuid();
            this.action = action;
        }

        public Guid Id { get; }

        public bool Matches(Topic topic)
        {
            return true;
        }

        public void Receive(IEvent evt)
        {
            this.action(evt);
        }
    }
}
