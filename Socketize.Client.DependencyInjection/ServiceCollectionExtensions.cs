using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Socketize.Client.Configuration;
using Socketize.Core.Abstractions;
using Socketize.Core.Routing;
using Socketize.Core.Services;
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
            services.AddSingleton(serviceProvider => CreateSchema(schemaConfig));
            AddSocketizeClientInternal(services, options);

            return services;
        }

        private static void AddSocketizeClientInternal(IServiceCollection services, ClientOptions options)
        {
            RegisterCommonServices(services, options);
            RegisterHandlers(services.BuildServiceProvider().GetService<Schema>(), services);
        }

        private static void RegisterCommonServices(this IServiceCollection services, ClientOptions options)
        {
            services.AddSingleton<IMessageHandlerFactory>(
                serviceProvider => new InjectedMessageHandlerFactory(serviceProvider));
            services.AddTransient<IProcessingService, ProcessingService>();
            services.AddSingleton<IPeer, ClientPeer>(serviceProvider => new ClientPeer(
                serviceProvider.GetService<IProcessingService>(),
                serviceProvider.GetService<ILogger<ClientPeer>>(),
                options));
        }

        private static Schema CreateSchema(Func<SchemaBuilder, SchemaBuilder> builderAction)
        {
            var schemaBuilder = SchemaBuilder.Create();

            return builderAction(schemaBuilder).Build();
        }

        private static void RegisterHandlers(Schema schema, IServiceCollection services)
        {
            foreach (var item in schema)
            {
                services.AddTransient(item.HandlerType);
            }
        }
    }
}