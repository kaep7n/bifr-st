using System;

namespace Bifröst.Core
{
    public interface ISubscription
    {
        Guid Id { get; }

        string Topic { get; }

        void Receive(IEvent evt);
    }
}
