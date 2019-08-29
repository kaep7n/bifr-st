
using System.Threading.Tasks;

namespace Bifröst.Core
{
    public interface IPublisher
    {
        Task PublishAsync(IEvent evt);
    }
}
