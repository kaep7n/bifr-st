using Bifröst.Playground.Events;
using Bifröst.Playground.Modules;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bifröst.Playground
{
    public class TransformData : Worker
    {
        public TransformData(ILogger<TransformData> logger, IBus bus)
            : base(logger, bus)
        {
            var pattern = new PatternBuilder("playground")
               .With("data")
               .With("rng")
               .Build();

            this.Subscription = new AsyncActionSubscription(this.TransformAsync, pattern);
        }

        private async Task TransformAsync(IEvent evt)
        {
            this.Logger.LogDebug($"transform: received evt with topic {evt.Topic}");
            
            var topic = new TopicBuilder("playground")
                         .With("data")
                         .With("ascii")
                         .Build();

            if (!(evt is ValueEvent valueEvent))
            {
                this.Logger.LogWarning($"transform: event is not from expected type ValueEvent");
                return;
            }

            this.Logger.LogDebug("transform: transforming value to ascii");
            var ascii = Convert.ToChar(valueEvent.Value);

            this.Logger.LogDebug($"transform: creating ascii event from transformed value {ascii}");
            var asciiEvent = new AsciiEvent(topic, ascii);
            
            this.Logger.LogDebug("transform: enqueuing event");
            await this.Bus.EnqueueAsync(asciiEvent)
                .ConfigureAwait(false);
        }
    }
}
