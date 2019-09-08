using System;
using System.Collections.Generic;
using System.Text;

namespace Bifröst.Core
{
    public interface IPublisherFactory
    {
        IPublisher Create();
    }
}
