using System;

namespace Bifröst
{
    public interface ISubscription
    {
        Guid Id { get; }

        bool Matches(Topic topic);

        void Receive(IEvent evt);
    }
}
