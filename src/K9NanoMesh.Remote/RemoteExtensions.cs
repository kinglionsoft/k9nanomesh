using Grpc.Core;
using K9NanoMesh.Core;
using K9NanoMesh.Remote;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using GRpcServer = Grpc.Core.Server;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RemoteExtensions
    {
        public static IServiceCollection AddGrpcClient(this IServiceCollection services)
        {
            services.AddSingleton<IGrpcConnection, GrpcConnection>();
            services.AddSingleton<IGrpcChannelFactory, GrpcChannelFactory>();
            return services;
        }

        public static IHostBuilder UseMagicOnionHost(this IHostBuilder builder, IApiInfo apiInfo)
        {
            return MagicOnion.Hosting.MagicOnionServerServiceExtension.UseMagicOnion(builder,
                new[] { new ServerPort(apiInfo.BindAddress, apiInfo.BindPort, ServerCredentials.Insecure) },
                new MagicOnionOptions());
        }

        public static IHost UseGrpcLogger(this IHost host, string categoryName = "grpc")
        {
            var logger = host.Services.GetService<ILoggerFactory>().CreateLogger(categoryName);
            GrpcEnvironment.SetLogger(new GrpcLogger(logger));
            return host;
        }

        public static IApplicationBuilder UseMagicOnionWebHost(this IApplicationBuilder app, IApiInfo apiInfo)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>() ??
                                      throw new ArgumentException("Missing Dependency", nameof(IApplicationLifetime));

            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("ServiceBuilder");

            var grpcServer = InitializeGrpcServer(apiInfo);

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                try
                {
                    grpcServer.ShutdownAsync().Wait();
                }
                catch (Exception ex)
                {
                    logger.LogError($"GrpcServer had shutown {ex}");
                }
                logger.LogInformation("Removing tenant & additional health check");
            });

            return app;
        }

        /// <summary>
        /// Initializing the GRPC service
        /// </summary>
        private static GRpcServer InitializeGrpcServer(IApiInfo apiInfo)
        {
            var grpcServer = new GRpcServer
            {
                Ports = { new ServerPort(apiInfo.BindAddress, apiInfo.BindPort, ServerCredentials.Insecure) },
                Services =
                {
                    MagicOnionEngine.BuildServerServiceDefinition()
                }
            };
            grpcServer.Start();
            return grpcServer;
        }
    }
}