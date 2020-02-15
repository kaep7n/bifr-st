using System.Threading.Tasks;

namespace Bifröst.Publishers
{
    public interface IPublisher
    {
        Task WriteAsync(IEvent evt);
    }
}
