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
        private readonly Timer timer = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds);
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
            this.Logger.LogDebug("get: data generation event elapsed");
            
            var topic = new TopicBuilder("playground")
                .With("data")
                .With("rng")
                .Build();

            this.Logger.LogDebug("get: generating data");
            var generatedData = this.GenerateRandomData().ToList();
            this.Logger.LogDebug("get: generating data completed");

            var watch = Stopwatch.StartNew();
            this.Logger.LogInformation($"get: enqueing {generatedData.Count} events");
            foreach (var data in generatedData)
            {
                this.Logger.LogDebug($"get: creating event for topic {topic} and data {data}");
                var evt = new ValueEvent(topic, data);

                this.Logger.LogDebug("get: enququing event");
                await this.Bus.EnqueueAsync(evt)
                    .ConfigureAwait(false);
            }

            this.Logger.LogInformation($"get: enqueing {generatedData.Count} events completed in {watch.ElapsedMilliseconds} ms");
        }

        private IEnumerable<int> GenerateRandomData()
        {
            var rng = new Random();

            for (var i = 0; i < 100_000; i++)
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
