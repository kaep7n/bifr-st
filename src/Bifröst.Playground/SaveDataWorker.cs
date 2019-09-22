using System;

namespace Bifröst.Playground
{
    public class SaveDataWorker
    {
        private readonly IBus bus;

        public SaveDataWorker(IBus bus)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            this.bus = bus;
        }
    }
}
