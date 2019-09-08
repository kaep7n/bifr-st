using System;

namespace Bifröst.Core
{
    public interface ISubscription
    {
        Guid Id { get; }

        bool Matches(Topic topic);


        void Receive(IEvent evt);
    }
}
