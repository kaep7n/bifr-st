using Bifröst.Subscriptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bifröst.Playground
{
    public class SaveData
    {
        private readonly AsyncActionSubscription subscription;
        private readonly ILogger logger;

        public SaveData(ILogger<SaveData> logger, AsyncActionSubscriptionFactory subscriptionFactory)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (subscriptionFactory is null)
            {
                throw new ArgumentNullException(nameof(subscriptionFactory));
            }

            var pattern = new PatternBuilder("playground")
               .With("data")
               .With("ascii")
               .Build();

            this.subscription = subscriptionFactory.Create(pattern, this.SimulateSaveData);
            this.logger = logger;
        }

        public void Enable() => this.subscription.Enable();

        public void Disable() => this.subscription.Disable();

        private async Task SimulateSaveData(IEvent evt)
        {
            this.logger.LogDebug($"save: received evt with topic {evt.Topic}");

            this.logger.LogDebug("save: delaying for 0 ms");
            await Task.Delay(0)
                .ConfigureAwait(false);

            this.logger.LogDebug("save: simulated save operation finished");
        }
    }
}
