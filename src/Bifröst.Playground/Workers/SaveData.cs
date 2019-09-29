using Bifröst.Playground.Events;
using Bifröst.Playground.Modules;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bifröst.Playground
{
    public class SaveData : Worker
    {
        public SaveData(ILogger<SaveData> logger, IBus bus)
            : base(logger, bus)
        {
            var pattern = new PatternBuilder("playground")
               .With("data")
               .With("ascii")
               .Build();

            this.subscription = new AsyncActionSubscription(this.SimulateSaveData, pattern);
        }

        private async Task SimulateSaveData(IEvent evt)
        {
            this.logger.LogDebug($"save: received evt with topic {evt.Topic}");

            this.logger.LogDebug("save: delaying for 20 ms");
            await Task.Delay(0)
                .ConfigureAwait(false);

            this.logger.LogDebug("save: simulated save operation finished");
        }
    }
}
