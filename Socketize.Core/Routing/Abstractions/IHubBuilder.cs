using System;

namespace Socketize.Core.Routing.Abstractions
{
    /// <summary>
    /// Base abstractions for any hub builder type.
    /// </summary>
    /// <typeparam name="TBuilder">Type of a scheme builder that implements this abstraction.</typeparam>
    public interface IHubBuilder<out TBuilder>
    {
        /// <summary>
        /// Adds hub (series of routes) to the schema builder.
        /// </summary>
        /// <param name="hubRoute">Route to the hub.</param>
        /// <param name="hubConfiguration">Delegate that describes inner routes and hubs.</param>
        /// <returns>Configured builder instance.</returns>
        TBuilder Hub(string hubRoute, Func<SchemaHubBuilder, SchemaHubBuilder> hubConfiguration);
    }
}