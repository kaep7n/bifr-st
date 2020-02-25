using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bifröst.Subscriptions
{
    public interface ISubscription
    {
        Guid Id { get; }

        bool IsEnabled { get; }
        
        Pattern Pattern { get; }

        Task EnableAsync(CancellationToken cancellationToken = default);

        Task DisableAsync(CancellationToken cancellationToken = default);
        
        bool Matches(Topic topic);
        
        Task WriteAsync(IEvent evt, CancellationToken cancellationToken = default);
    }
}