using System;
using System.Threading.Tasks;

namespace Bifröst.Playground
{
    public class WriteToConsoleWorker
    {
        private readonly IBus bus;
        private readonly AsyncActionSubscription subscription;

        public WriteToConsoleWorker(IBus bus)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.bus = bus;

            var topic = new TopicBuilder("playground")
               .With("data")
               .With("ascii")
               .Build();

            this.subscription = new AsyncActionSubscription(this.WriteToConsoleAsync, topic);
            this.bus.Subscribe(this.subscription);
            this.subscription.Enable();
        }

        private Task WriteToConsoleAsync(IEvent evt)
        {
            Console.Write((evt as AsciiEvent).Ascii);

            return Task.CompletedTask;
        }
    }
}
