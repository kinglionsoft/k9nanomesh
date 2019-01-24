using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using K9NanoMesh.Core;
using MagicOnion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace K9NanoMesh.Remote
{
    public interface IGrpc<TService> where TService: IService<TService>
    {
        Task<TService> GetAsync();
    }

    public class GrpcServiceAccessor<TService> : IGrpc<TService> where TService : IService<TService>
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<Type, IApiInfo> _apiInfos;

        public GrpcServiceAccessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _apiInfos = new ConcurrentDictionary<Type, IApiInfo>();
        }

        public Task<TService> GetAsync()
        {
            var apiInfo = _apiInfos.GetOrAdd(typeof(TService), type =>
            {
                var apiInfoType = type.Assembly.GetTypes()
                    .FirstOrDefault(x=> !x.IsAbstract && x.IsClass && x.GetInterfaces().Contains(typeof(IApiInfo)));
                if (apiInfoType == null)
                {
                    throw new ArchitectureException($"Cannot find any implementation class of IApiInfo on {type.Assembly}.");
                }

                var ctor = apiInfoType.GetConstructor(new []{typeof(IConfiguration)});
                if (ctor != null)
                {
                    // Construct with IConfiguration
                    var configuration = _serviceProvider.GetService<IConfiguration>();
                    return (IApiInfo)Activator.CreateInstance(apiInfoType, configuration);
                }

                ctor = apiInfoType.GetConstructor(new Type[0]);
                if (ctor != null)
                {
                    // Construct with default parameterless constructor
                    return (IApiInfo) Activator.CreateInstance(apiInfoType);
                }

                throw new ArchitectureException($"{apiInfoType} must have either a parameterless constructor or a constructor with IConfiguration.");
            });

            return _serviceProvider
                .GetService<IGrpcConnection>()
                .GetRemoteService<TService>(apiInfo.BindAddress, apiInfo.BindPort);
        }
    }
}