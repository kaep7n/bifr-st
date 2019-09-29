using Bifröst.Playground.Events;
using Bifröst.Playground.Modules;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bifröst.Playground
{
    public class TransformData : Module
    {
        static int callCount = 0;

        public TransformData(ILogger<TransformData> logger, IBus bus)
            : base(logger, bus)
        {
            var pattern = new PatternBuilder("playground")
               .With("data")
               .With("rng")
               .Build();

            this.subscription = new AsyncActionSubscription(this.TransformAsync, pattern);
        }

        private async Task TransformAsync(IEvent evt)
        {
            this.logger.LogDebug($"transform: received evt with topic {evt.Topic}");
            
            var topic = new TopicBuilder("playground")
                         .With("data")
                         .With("ascii")
                         .Build();

            if (!(evt is ValueEvent valueEvent))
            {
                this.logger.LogWarning($"transform: event is not from expected type ValueEvent");
                return;
            }

            this.logger.LogDebug("transform: transforming value to ascii");
            var ascii = Convert.ToChar(valueEvent.Value);

            this.logger.LogDebug($"transform: creating ascii event from transformed value {ascii}");
            var asciiEvent = new AsciiEvent(topic, ascii);
            
            this.logger.LogDebug("transform: enqueuing event");
            await this.bus.EnqueueAsync(asciiEvent);

            callCount++;
            this.logger.LogInformation($"transform: called {callCount} times");
        }
    }
}
