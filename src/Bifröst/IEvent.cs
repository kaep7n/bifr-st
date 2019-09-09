using System;

namespace Bifröst
{
    public interface IEvent
    {
        Guid Id { get; }

        Topic Topic { get; }
    }
}
