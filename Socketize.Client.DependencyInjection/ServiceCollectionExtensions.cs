using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Socketize.Client.Configuration;
using Socketize.Core.Abstractions;
using Socketize.Core.Routing;
using Socketize.Core.Serialization;
using Socketize.Core.Serialization.Abstractions;
using Socketize.Core.Services.Abstractions;
using Socketize.DependencyInjection;

namespace Socketize.Client.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="IServiceCollection"/> type.
    /// </summary>
    // TODO: https://github.com/seclerp/Socketize/issues/5: Implement better setup API for Client
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Socketize client to dependency injection container.
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="schemaConfig">Delegate that configures schema.</param>
        /// <param name="options">Server configuration options instance.</param>
        /// <returns>Configured instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSocketizeClient(
            this IServiceCollection services,
            Func<SchemaBuilder, SchemaBuilder> schemaConfig,
            ClientOptions options)
        {
            var schemaBuilder = SchemaBuilder.Create();
            var schema = schemaConfig(schemaBuilder).Build();

            services.AddSocketizeCommons(schema);
            services.AddTransient<IDtoSerializer, MessagePackDtoSerializer>();
            services.AddSingleton(serviceProvider => new ClientPeer(
                serviceProvider.GetService<IProcessingService>(),
                serviceProvider.GetService<IDtoSerializer>(),
                serviceProvider.GetService<ILogger<ClientPeer>>(),
                options));
            services.AddSingleton<IPeer, ClientPeer>(serviceProvider => serviceProvider.GetService<ClientPeer>());

            return services;
        }

        /// <summary>
        /// Adds Socketize client to dependency injection container.
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="schemaConfig">Delegate that configures schema.</param>
        /// <param name="options">Server configuration options instance.</param>
        /// <param name="serializerFactory">Factory that provides DTO serializer instance.</param>
        /// <returns>Configured instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSocketizeClient(
            this IServiceCollection services,
            Func<SchemaBuilder, SchemaBuilder> schemaConfig,
            ClientOptions options,
            Func<IServiceProvider, IDtoSerializer> serializerFactory)
        {
            var schemaBuilder = SchemaBuilder.Create();
            var schema = schemaConfig(schemaBuilder).Build();

            services.AddSocketizeCommons(schema);
            services.AddSingleton(serviceProvider => new ClientPeer(
                serviceProvider.GetService<IProcessingService>(),
                serializerFactory(serviceProvider),
                serviceProvider.GetService<ILogger<ClientPeer>>(),
                options));
            services.AddSingleton<IPeer, ClientPeer>(serviceProvider => serviceProvider.GetService<ClientPeer>());

            return services;
        }

        /// <summary>
        /// Adds Socketize client to dependency injection container.
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="options">Server configuration options instance.</param>
        /// <returns>Configured instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSocketizeClient(
            this IServiceCollection services,
            ClientOptions options)
        {
            return AddSocketizeClient(services, _ => _, options);
        }

        /// <summary>
        /// Adds Socketize client to dependency injection container.
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="options">Server configuration options instance.</param>
        /// <param name="serializerFactory">Factory that provides DTO serializer instance.</param>
        /// <returns>Configured instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSocketizeClient(
            this IServiceCollection services,
            ClientOptions options,
            Func<IServiceProvider, IDtoSerializer> serializerFactory)
        {
            return AddSocketizeClient(services, _ => _, options, serializerFactory);
        }
    }
}