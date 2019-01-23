using System.Threading.Tasks;
using MagicOnion;

namespace K9NanoMesh.Remote
{
    public interface IGrpcConnection
    {
        ///  <summary>
        ///  Get the specified remote service interface
        ///  </summary>
        /// <typeparam name="TService">Remote Service Interface type</typeparam>
        /// <param name = "serviceName">Remote Service Name</param>
        ///  <param name="servicePort">Remote Service Port</param>
        Task<TService> GetRemoteService<TService>(string serviceName, int servicePort) where TService : IService<TService>;
    }
}