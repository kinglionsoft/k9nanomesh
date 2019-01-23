using K9NanoMesh.Remote;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleService.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sample.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddGrpcClient()
                .BuildServiceProvider();

            var grpcChannel = serviceProvider.GetRequiredService<IGrpcConnection>();

            var service = grpcChannel.GetRemoteService<IAddService>("localhost", 9901).Result;
            var r = await service.SumAsync(1, 2);
            Console.WriteLine($"1 + 2 = {r}");
            Console.ReadKey();
        }
    }
}
