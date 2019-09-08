
using System;

namespace Bifröst.Core
{
    public interface IEvent
    {
        Guid Id { get; }

        Topic Topic { get; }
    }
}
