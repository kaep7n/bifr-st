using System;
using System.Threading.Tasks;

namespace Bifröst
{
    public interface ISubscription
    {
        Guid Id { get; }

        bool Matches(Topic topic);

        Task EnqueueAsync(IEvent evt);
    }
}
