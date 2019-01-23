using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Server;
using SampleService.Abstractions;

namespace Sample.Host
{
    public class AddService: ServiceBase<IAddService>, IAddService
    {
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            await Task.CompletedTask;
            return x + y;
        }
    }
}