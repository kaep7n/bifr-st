using System.Collections.Generic;

namespace Bifröst
{
    public interface IMetrics
    {
        IEnumerable<Metric> GetMetrics();
    }
}
