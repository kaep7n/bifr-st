using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bifröst
{
    public interface ISubscription
    {
        Guid Id { get; }

        bool IsEnabled { get; }
        
        IEnumerable<Topic> Topics { get; }

        void Disable();
        
        void Enable();
        
        bool Matches(Topic topic);
        
        Task EnqueueAsync(IEvent evt);
    }
}