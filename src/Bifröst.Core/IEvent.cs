
using System;

namespace Bifröst.Core
{
    public interface IEvent
    {
        Guid Id { get; }

        string Topic { get; }
    }
}
