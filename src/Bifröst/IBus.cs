using Bifröst.Subscriptions;
using System.Threading;
using System.Threading.Tasks;

namespace Bifröst
{
    public interface IBus
    {
        bool IsRunning { get; }

        Task RunAsync(CancellationToken cancellationToken = default);

        Task IdleAsync(CancellationToken cancellationToken = default);

        void Subscribe(ISubscription subscription);

        void Unsubscribe(ISubscription subscription);

        Task WriteAsync(IEvent evt);
    }
}