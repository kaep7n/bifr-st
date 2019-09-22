using System;
using System.Threading.Tasks;

namespace Bifröst.Playground
{
    public class TransformDataWorker
    {
        private readonly IBus bus;
        private AsyncActionSubscription subscription;

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

        private Task TransformAsync(IEvent evt)
        {
            Console.WriteLine($"received event {evt.Topic.Path} ({evt.Id})");
            return Task.CompletedTask;
        }
    }
}
