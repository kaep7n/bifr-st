using System.Collections.Generic;

namespace Bifröst
{
    public interface IMetricsProvider
    {
        IEnumerable<Metric> GetMetrics();
    }
}
