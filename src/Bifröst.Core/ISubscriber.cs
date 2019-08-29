
using System.Threading.Tasks;

namespace Bifröst.Core
{
    public interface ISubscriber
    {
        Task SubscribeAsync();
    }
}
