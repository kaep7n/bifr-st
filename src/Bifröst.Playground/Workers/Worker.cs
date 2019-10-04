using Microsoft.Extensions.Logging;
using System;

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

        public virtual void Start()
        {
            this.Logger.LogInformation("starting module");

            if (this.Subscription == null)
            {
                return;
            }

            this.Logger.LogInformation("enabling subscription");
            this.Subscription.Enable();

            this.Logger.LogInformation("subscribing to bus");
            this.Bus.Subscribe(this.Subscription);
        }

        public virtual void Stop()
        {
            this.Logger.LogInformation("stopping module");

            if(this.Subscription == null)
            {
                return;
            }

            this.Logger.LogInformation("enabling subscription");
            this.Subscription.Disable();

            this.Logger.LogInformation("unsubscribing from bus");
            this.Bus.Unsubscribe(this.Subscription);
        }
    }
}
