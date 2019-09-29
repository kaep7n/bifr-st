using Microsoft.Extensions.Logging;
using System;

namespace Bifröst.Playground.Modules
{
    public class Worker
    {
        protected readonly ILogger logger;
        protected readonly IBus bus;

        protected AsyncActionSubscription subscription;

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

            this.logger = logger;
            this.bus = bus;
        }

        public virtual void Start()
        {
            this.logger.LogInformation("starting module");

            if (this.subscription == null)
            {
                return;
            }

            this.logger.LogInformation("enabling subscription");
            this.subscription.Enable();

            this.logger.LogInformation("subscribing to bus");
            this.bus.Subscribe(this.subscription);
        }

        public virtual void Stop()
        {
            this.logger.LogInformation("stopping module");

            this.logger.LogInformation("enabling subscription");
            this.subscription.Disable();

            this.logger.LogInformation("unsubscribing from bus");
            this.bus.Unsubscribe(this.subscription);
        }
    }
}
