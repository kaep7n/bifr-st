using System;
using System.Threading.Tasks;

namespace Bifröst.Subscriptions
{
    public interface ISubscription
    {
        Guid Id { get; }

        bool IsEnabled { get; }
        
        Pattern Pattern { get; }

        void Disable();
        
        void Enable();
        
        bool Matches(Topic topic);
        
        Task EnqueueAsync(IEvent evt);
    }
}