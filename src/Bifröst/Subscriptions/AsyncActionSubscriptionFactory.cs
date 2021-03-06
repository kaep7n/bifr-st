﻿using System;
using System.Threading.Tasks;

namespace Bifröst.Subscriptions
{
    public class AsyncActionSubscriptionFactory
    {
        private readonly IBus bus;

        public AsyncActionSubscriptionFactory(IBus bus)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.bus = bus;
        }

        public AsyncActionSubscription Create(Pattern pattern, Func<IEvent, Task> func) => new AsyncActionSubscription(this.bus, pattern, func);
    }
}
