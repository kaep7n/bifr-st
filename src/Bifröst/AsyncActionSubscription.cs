using System;
using System.Threading.Tasks;

namespace Bifröst
{
    public class AsyncActionSubscription : ISubscription
    {
        private readonly Func<IEvent, Task> asyncAction;

        public AsyncActionSubscription(Func<IEvent, Task> asyncAction)
        {
            if (asyncAction is null)
            {
                throw new ArgumentNullException(nameof(asyncAction));
            }

            this.Id = Guid.NewGuid();
            this.asyncAction = asyncAction;
        }

        public Guid Id { get; }

        public bool Matches(Topic topic)
        {
            return true;
        }

        public async Task EnqueueAsync(IEvent evt)
        {
            await this.asyncAction(evt)
                .ConfigureAwait(false);
        }
    }
}
