using Bifröst.Subscriptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bifröst.Playground.Modules
{
    public class Worker
    {
        protected ILogger Logger { get; }

        protected IBus Bus { get; }

        protected AsyncActionSubscription Subscription { get; set; }

        public Worker(ILogger logger, IBus bus)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.Logger = logger;
            this.Bus = bus;
        }

        public virtual async Task EnableAsync()
        {
            this.Logger.LogInformation("starting worker");

            if (this.Subscription == null)
            {
                return;
            }

            this.Logger.LogInformation("enabling subscription");
            await this.Subscription.EnableAsync().ConfigureAwait(false);

            this.Logger.LogInformation("subscribing to bus");
            this.Bus.Subscribe(this.Subscription);
        }

        public virtual async Task DisableAsync()
        {
            this.Logger.LogInformation("stopping worker");

            if(this.Subscription == null)
            {
                return;
            }

            this.Logger.LogInformation("enabling subscription");
            await this.Subscription.DisableAsync().ConfigureAwait(false);

            this.Logger.LogInformation("unsubscribing from bus");
            this.Bus.Unsubscribe(this.Subscription);
        }
    }
}
