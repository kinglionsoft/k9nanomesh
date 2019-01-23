﻿using MagicOnion;
using MagicOnion.Server;

namespace K9NanoMesh.Remote.Test
{
    public interface ITestService : IService<ITestService>
    {
        UnaryResult<int> Sum(int x, int y);
    }

    public class TestServiceImpl : ServiceBase<ITestService>, ITestService
    {
        public async UnaryResult<int> Sum(int x, int y)
        {
            return x + y;
        }
    }
}