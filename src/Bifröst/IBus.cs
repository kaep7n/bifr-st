using Bifröst.Subscriptions;
using System.Threading.Tasks;

namespace Bifröst
{
    public interface IBus
    {
        bool IsRunning { get; }

        Task RunAsync();

        Task IdleAsync();

        void Subscribe(ISubscription subscription);

        void Unsubscribe(ISubscription subscription);

        Task WriteAsync(IEvent evt);
    }
}