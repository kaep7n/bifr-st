using System.Threading.Tasks;

namespace Bifröst.Core
{
    public class Publisher : IPublisher
    {
        public Task PublishAsync(IEvent evt)
        {
            return Task.CompletedTask;
        }
    }
}
