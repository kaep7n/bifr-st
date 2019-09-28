using System;
using System.Threading.Tasks;

namespace Bifröst
{
    public class AsyncActionSubscription : Subscription
    {
        private readonly Func<IEvent, Task> asyncAction;

        public AsyncActionSubscription(Func<IEvent, Task> asyncAction, Pattern pattern)
            : base(pattern)
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

            await this.asyncAction(evt).ConfigureAwait(false);
        }
    }
}
