using Bifröst.Subscriptions;
using System.Threading.Tasks;

namespace Bifröst
{
    public interface IBus
    {
        bool IsRunning { get; }
        
        long WaitingEventCount { get; }

        long ProcessedEventCount { get; }

        long FailedEventCount { get; }

        void Run();
        
        void Idle();
        
        void Subscribe(ISubscription subscription);
        
        void Unsubscribe(ISubscription subscription);

        Task WriteAsync(IEvent evt);
    }
}