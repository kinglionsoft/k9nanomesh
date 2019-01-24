using MagicOnion;
using MagicOnion.Server;
using SampleService.Abstractions;

namespace K9NanoMesh.Remote.Test
{
    public class SampleService: ServiceBase<IAddService>, IAddService
    {
        public UnaryResult<int> SumAsync(int x, int y)
        {
            return new UnaryResult<int>(x + y);
        }
    }
}