using System;
using System.Collections.Generic;
using System.Timers;

namespace Bifröst.Playground
{
    public class GetDataWorker
    {
        private readonly Timer timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
        private readonly IBus bus;

        public GetDataWorker(IBus bus)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.bus = bus;
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.Start();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var topic = new TopicBuilder("playground")
                .With("data")
                .With("rng")
                .Build();

            Console.WriteLine("getting data...");

            foreach (var data in this.GetData())
            {
                var evt = new DataEvent(topic, data);
                await this.bus.EnqueueAsync(evt);
            }

            Console.WriteLine("sent data to bus");
        }

        private IEnumerable<Data> GetData()
        {
            var rng = new Random();

            for (var i = 0; i < 10; i++)
            {
                yield return new Data("rng", rng.Next(65, 122));
            }
        }
    }
}
