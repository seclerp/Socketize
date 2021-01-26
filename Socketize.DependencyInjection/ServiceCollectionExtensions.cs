using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Socketize.Core.Abstractions;
using Socketize.Core.Enums;
using Socketize.Core.Routing;
using Socketize.Core.Services;
using Socketize.Core.Services.Abstractions;

namespace Socketize.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds schema, common services and class handlers to the dependency injection container.
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="schema">Instance of <see cref="Schema"/>.</param>
        /// <returns>Configured instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSocketizeCommons(this IServiceCollection services, Schema schema)
        {
            services.AddSingleton(schema);
            services.RegisterCommonServices();
            services.RegisterClassHandlers(schema.Where(item => item.Kind is HandlerInstanceKind.Class));

            return services;
        }

        private static IServiceCollection RegisterCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<IMessageHandlerFactory>(
                serviceProvider => new InjectedMessageHandlerFactory(serviceProvider));
            services.AddSingleton<IMessageHandlersManager, MessageHandlersManager>();
            services.AddTransient<IProcessingService, ProcessingService>();

            return services;
        }

        private static IServiceCollection RegisterClassHandlers(this IServiceCollection services, IEnumerable<SchemaItem> schemaItems)
        {
            foreach (var item in schemaItems)
            {
                services.AddTransient(item.Handler as Type);
            }

            return services;
        }
    }
}