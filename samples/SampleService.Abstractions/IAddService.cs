using System;
using MagicOnion;
using MagicOnion.Server;

namespace SampleService.Abstractions
{
    public interface IAddService: IService<IAddService>
    {
        UnaryResult<int> SumAsync(int x, int y);
    }
}
