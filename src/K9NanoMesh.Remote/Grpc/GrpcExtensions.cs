using Grpc.Core;
using K9NanoMesh.Core;
using K9NanoMesh.Remote;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using GRpcServer = Grpc.Core.Server;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GrpcExtensions
    {
        public static IServiceCollection AddGrpcClient(this IServiceCollection services)
        {
            services.AddSingleton<IGrpcConnection, GrpcConnection>();
            services.AddSingleton<IGrpcChannelFactory, GrpcChannelFactory>();
            services.AddSingleton(typeof(IGrpc<>), typeof(GrpcServiceAccessor<>));

            return services;
        }

        public static IHostBuilder UseMagicOnionHost(this IHostBuilder builder,
            IApiInfo apiInfo,
            Assembly[] searchAssemblies = null,
            MagicOnionOptions options = null,
            IEnumerable<ChannelOption> channelOptions = null)
        {
            if (searchAssemblies == null)
            {
                searchAssemblies = new[]
                {
                    Assembly.GetEntryAssembly()
                };
            }
            return MagicOnion.Hosting.MagicOnionServerServiceExtension.UseMagicOnion(builder,
                new[] { new ServerPort(apiInfo.BindAddress, apiInfo.BindPort, ServerCredentials.Insecure) },
                options ?? new MagicOnionOptions(),
                null,
                searchAssemblies,
                channelOptions);
        }

        public static IApplicationBuilder UseMagicOnionWebHost(this IApplicationBuilder app, 
            IApiInfo apiInfo, 
            Assembly[] searchAssemblies = null)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>() ??
                                      throw new ArchitectureException($"Missing Dependency: {nameof(IApplicationLifetime)}");

            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(apiInfo.ApiName);
            GrpcEnvironment.SetLogger(new GrpcLogger(logger));

            var grpcServer = InitializeGrpcServer(apiInfo, searchAssemblies);

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                try
                {
                    grpcServer.ShutdownAsync().Wait();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"GrpcServer had shutdown");
                }
                logger.LogInformation("GrpcServer had shutdown");
            });

            return app;
        }

        /// <summary>
        /// Initializing the GRPC service
        /// </summary>
        private static GRpcServer InitializeGrpcServer(IApiInfo apiInfo, Assembly[] searchAssemblies)
        {
            var option = new MagicOnionOptions
            {
#if DEBUG
                IsReturnExceptionStackTraceInErrorDetail = true
#else
                IsReturnExceptionStackTraceInErrorDetail = false
#endif
            };

            if (searchAssemblies == null)
            {
                searchAssemblies = new[]
                {
                    Assembly.GetEntryAssembly(),
                };
            }

            var grpcServer = new GRpcServer
            {
                Ports = { new ServerPort(apiInfo.BindAddress, apiInfo.BindPort, ServerCredentials.Insecure) },
                Services =
                {

                    MagicOnionEngine.BuildServerServiceDefinition(
                        searchAssemblies, option)
                }
            };
            grpcServer.Start();
            return grpcServer;
        }
    }
}