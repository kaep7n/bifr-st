using System;
using System.Collections.Generic;
using System.Text;

namespace Bifröst.Playground
{
    public class Hub
    {
        private readonly IBus bus;
        private readonly GetDataWorker getData;
        private readonly TransformDataWorker transformDataWorker;
        private readonly WriteToConsoleWorker saveDataWorker;

        public Hub(IBus bus, GetDataWorker getData, TransformDataWorker transformDataWorker, WriteToConsoleWorker saveDataWorker)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            if (getData is null)
            {
                throw new ArgumentNullException(nameof(getData));
            }

            if (transformDataWorker is null)
            {
                throw new ArgumentNullException(nameof(transformDataWorker));
            }

            if (saveDataWorker is null)
            {
                throw new ArgumentNullException(nameof(saveDataWorker));
            }

            this.bus = bus;
            this.bus.Start();
            this.getData = getData;
            this.transformDataWorker = transformDataWorker;
            this.saveDataWorker = saveDataWorker;
        }
    }
}
