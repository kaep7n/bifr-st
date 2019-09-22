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

            var dataEvent = evt as DataEvent;

            Console.WriteLine($"received event {evt.Topic.Path} ({evt.Id})");

            var data = dataEvent.Data;
            var ascii = Convert.ToChar(data.Value);

            var asciiEvent = new AsciiEvent(topic, data.Key, ascii);

            await this.bus.EnqueueAsync(asciiEvent);
        }
    }
}
