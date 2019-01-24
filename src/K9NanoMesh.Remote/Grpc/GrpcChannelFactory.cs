using System.Collections.Concurrent;
using Grpc.Core;

namespace K9NanoMesh.Remote
{
    public class GrpcChannelFactory : IGrpcChannelFactory
    {
        private readonly ConcurrentDictionary<string, Channel> _grpcServers;

        public GrpcChannelFactory()
        {
            _grpcServers = new ConcurrentDictionary<string, Channel>();
        }

        public Channel Get(string address, int port)
        {
            var key = $"{address}:{port}";
            var channel = _grpcServers.GetOrAdd(key, new Channel(address, port, ChannelCredentials.Insecure));
            return channel;
        }
    }
}