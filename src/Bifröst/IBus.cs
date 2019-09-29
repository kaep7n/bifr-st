using System.Threading.Tasks;

namespace Bifröst
{
    public interface IBus
    {
        bool IsRunning { get; }
        
        void Start();
        
        void Stop();
        
        void Subscribe(ISubscription subscription);
        
        void Unsubscribe(ISubscription subscription);

        Task EnqueueAsync(IEvent evt);
    }
}