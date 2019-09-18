using System.Threading.Tasks;

namespace Bifröst
{
    public interface IPublisher
    {
        Task PublishAsync(IEvent evt);
    }
}
