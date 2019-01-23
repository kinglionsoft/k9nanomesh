using System;
using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;

namespace K9NanoMesh.Remote
{
    public class GrpcConnection: IGrpcConnection
    {
        private readonly IGrpcChannelFactory _grpcChannelFactory;

        public GrpcConnection(IGrpcChannelFactory grpcChannelFactory)
        {
            _grpcChannelFactory = grpcChannelFactory;
        }

        public Task<TService> GetRemoteService<TService>(string serviceName, int servicePort) 
            where TService : IService<TService>
        {
            var channel = _grpcChannelFactory.Get(serviceName, servicePort);
            var service = MagicOnionClient.Create<TService>(channel);
            return Task.FromResult(service);
        }
    }
}