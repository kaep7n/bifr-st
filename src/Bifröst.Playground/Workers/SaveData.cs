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

            this.Subscription = new AsyncActionSubscription(this.SimulateSaveData, pattern);
        }

        private async Task SimulateSaveData(IEvent evt)
        {
            this.Logger.LogDebug($"save: received evt with topic {evt.Topic}");

            this.Logger.LogDebug("save: delaying for 0 ms");
            await Task.Delay(0)
                .ConfigureAwait(false);

            this.Logger.LogDebug("save: simulated save operation finished");
        }
    }
}
