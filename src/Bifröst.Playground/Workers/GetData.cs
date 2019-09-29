using Bifröst.Playground.Events;
using Bifröst.Playground.Modules;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace Bifröst.Playground
{
    public class GetData : Worker, IDisposable
    {
        private readonly Timer timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
        private bool isDisposed = false;

        public GetData(ILogger<GetData> logger, IBus bus)
            : base(logger, bus)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.timer.Elapsed += this.Timer_Elapsed;
        }

        public override void Start()
        {
            base.Start();
            this.timer.Start();
        }

        public override void Stop()
        {
            this.timer.Stop();
            base.Stop();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.logger.LogDebug("get: data generation event elapsed");
            
            var topic = new TopicBuilder("playground")
                .With("data")
                .With("rng")
                .Build();


            this.logger.LogDebug("get: generating data");
            var generatedData = this.GenerateRandomData().ToList();
            this.logger.LogDebug("get: generating data completed");

            var watch = Stopwatch.StartNew();
            this.logger.LogInformation($"get: enqueing {generatedData.Count} events");
            foreach (var data in generatedData)
            {
                this.logger.LogDebug($"get: creating event for topic {topic} and data {data}");
                var evt = new ValueEvent(topic, data);

                this.logger.LogDebug("get: enququing event");
                await this.bus.EnqueueAsync(evt)
                    .ConfigureAwait(false);
            }

            this.logger.LogInformation($"get: enqueing {generatedData.Count} events completed in {watch.ElapsedMilliseconds} ms");
        }

        private IEnumerable<int> GenerateRandomData()
        {
            var rng = new Random();

            for (var i = 0; i < 100; i++)
            {
                yield return rng.Next(65, 122);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.timer.Dispose();
                }

                this.isDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
