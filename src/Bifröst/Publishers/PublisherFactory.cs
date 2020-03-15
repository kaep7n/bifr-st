using System;

namespace Bifröst.Publishers
{
    public class PublisherFactory
    {
        private readonly IBus bus;

        public PublisherFactory(IBus bus)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.bus = bus;
        }

        public IPublisher Create() => new Publisher(this.bus);
    }
}
