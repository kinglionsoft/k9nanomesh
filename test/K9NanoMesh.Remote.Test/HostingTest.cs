using Grpc.Core;
using MagicOnion.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using K9NanoMesh.Core;

namespace K9NanoMesh.Remote.Test
{
    public class HostingTest
    {
        [Fact]
        public async Task TestRemoteService()
        {
            IApiInfo apiInfo = new TestApiInfo();
            var host = new HostBuilder()
                .UseMagicOnionHost(apiInfo, searchAssemblies: new [] { typeof(ITestService).Assembly } )
                .ConfigureServices((hostContext, service) =>
                {
                    service.AddGrpcClient();
                })
                .Build();
            using (host)
            {
                host.Start();

                var grpcChannel = host.Services.GetRequiredService<IGrpcConnection>();
                var service = await grpcChannel.GetRemoteService<ITestService>(apiInfo.BindAddress, apiInfo.BindPort);

                for (var i = 0; i < 10; i++)
                {
                    var ret = await service.Sum(i, i);
                    Assert.Equal(i * 2, ret);
                }
                await host.StopAsync();
            }
        }
    }
}
