using System.Threading.Tasks;

namespace Bifröst.Publishers
{
    public interface IPublisher
    {
        Task PublishAsync(IEvent evt);
    }
}
