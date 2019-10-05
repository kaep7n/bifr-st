using System;
using System.Threading.Tasks;

namespace Bifröst.Subscriptions
{
    public class AsyncActionSubscription : Subscription
    {
        private readonly Func<IEvent, Task> asyncAction;

        public AsyncActionSubscription(IBus bus, Pattern pattern, Func<IEvent, Task> asyncAction)
            : base(bus, pattern)
        {
            if (asyncAction is null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }

            this.asyncAction = asyncAction;
        }

        protected override async Task ProcessEventAsync(IEvent evt)
        {
            if (evt is null)
            {
                throw new ArgumentNullException(nameof(evt));
            }

            await this.asyncAction(evt)
                .ConfigureAwait(false);
        }
    }
}
