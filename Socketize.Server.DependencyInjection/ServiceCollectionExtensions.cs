using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Socketize.Core.Abstractions;
using Socketize.Core.Routing;
using Socketize.Core.Services.Abstractions;
using Socketize.DependencyInjection;
using Socketize.Server.Configuration;

namespace Socketize.Server.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Socketize to dependency injection container.
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="schemaConfig">Delegate that configures schema.</param>
        /// <param name="options">Server configuration options instance.</param>
        /// <returns>Configured instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSocketizeServer(
            this IServiceCollection services,
            Func<SchemaBuilder, SchemaBuilder> schemaConfig,
            ServerOptions options)
        {
            var schemaBuilder = SchemaBuilder.Create();
            var schema = schemaConfig(schemaBuilder).Build();

            services.AddSocketizeCommons(schema);
            services.AddSingleton<IPeer, ServerPeer>(serviceProvider => new ServerPeer(
                serviceProvider.GetService<IProcessingService>(),
                serviceProvider.GetService<ILogger<ServerPeer>>(),
                options));

            return services;
        }
    }
}