using System;
using System.Threading.Tasks;

namespace Bifröst.Publishers
{
    public class Publisher : IPublisher
    {
        private readonly IBus bus;

        public Publisher(IBus bus)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.bus = bus;
        }

        public async Task WriteAsync(IEvent evt)
        {
            if (evt is null)
            {
                throw new ArgumentNullException(nameof(evt));
            }

            await this.bus.WriteAsync(evt)
                .ConfigureAwait(false);
        }
    }
}
