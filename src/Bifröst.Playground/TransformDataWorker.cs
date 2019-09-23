using System;
using System.Text;
using System.Threading.Tasks;

namespace Bifröst.Playground
{
    public class TransformDataWorker
    {
        private readonly IBus bus;
        private readonly AsyncActionSubscription subscription;

        public TransformDataWorker(IBus bus)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.bus = bus;

            var topic = new TopicBuilder("playground")
               .With("data")
               .With("rng")
               .Build();

            this.subscription = new AsyncActionSubscription(this.TransformAsync, topic);
            this.bus.Subscribe(this.subscription);
            this.subscription.Enable();
        }

        private async Task TransformAsync(IEvent evt)
        {
            var topic = new TopicBuilder("playground")
                         .With("data")
                         .With("ascii")
                         .Build();

            var valueEvent = evt as ValueEvent;

            var ascii = Convert.ToChar(valueEvent.Value);
            var asciiEvent = new AsciiEvent(topic, ascii);

            Console.WriteLine($"transformed {valueEvent.Value} to {asciiEvent.Ascii}");

            await this.bus.EnqueueAsync(asciiEvent);
        }
    }
}
