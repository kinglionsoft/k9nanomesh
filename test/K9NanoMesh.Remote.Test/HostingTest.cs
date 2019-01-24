using Grpc.Core;
using MagicOnion.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using K9NanoMesh.Core;
using SampleService.Abstractions;

namespace K9NanoMesh.Remote.Test
{
    public class HostingTest
    {
        [Fact]
        public async Task TestRemoteService()
        {
            IApiInfo apiInfo = new SampleApiInfo();
            var host = new HostBuilder()
                .UseMagicOnionHost(apiInfo, new Assembly[]{ typeof(HostingTest).Assembly})
                .ConfigureServices((hostContext, service) =>
                {
                    service.AddGrpcClient();
                })
                .Build();
            using (host)
            {
                host.Start();

                var serviceAccessor = host.Services.GetService<IGrpc<IAddService>>();
                var service = await serviceAccessor.GetAsync();

                for (var i = 0; i < 10; i++)
                {
                    var ret = await service.SumAsync(i, i);
                    Assert.Equal(i * 2, ret);
                }
                await host.StopAsync();
            }
        }
    }
}
