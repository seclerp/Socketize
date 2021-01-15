using Microsoft.Extensions.DependencyInjection;

namespace Socketize.Server.AspNetCore
{
    /// <summary>
    /// Extensions for <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Socketize server hosted service to host configured Socketize server.
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/>.</param>
        /// <returns>Configured instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSocketizeHosting(this IServiceCollection services)
        {
            services.AddHostedService<ServerHostedService>();

            return services;
        }
    }
}