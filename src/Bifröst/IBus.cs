using Bifröst.Subscriptions;
using System.Threading.Tasks;

namespace Bifröst
{
    public interface IBus
    {
        bool IsRunning { get; }
        
        void Run();
        
        void Idle();
        
        void Subscribe(ISubscription subscription);
        
        void Unsubscribe(ISubscription subscription);

        Task EnqueueAsync(IEvent evt);
    }
}