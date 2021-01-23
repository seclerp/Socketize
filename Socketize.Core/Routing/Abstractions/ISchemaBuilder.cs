using System;
using Socketize.Core.Abstractions;

namespace Socketize.Core.Routing.Abstractions
{
    /// <summary>
    /// Base abstractions for any scheme builder type.
    /// </summary>
    /// <typeparam name="TBuilder">Type of a scheme builder that implements this abstraction.</typeparam>
    public interface ISchemaBuilder<TBuilder>
    {
        /// <summary>
        /// Adds message handler for a given route to the schema builder.
        /// </summary>
        /// <param name="route">Route to add message handler to.</param>
        /// <typeparam name="TMessageHandler">Type of a message handler to use for this route.</typeparam>
        /// <returns>Configured builder instance.</returns>
        TBuilder Route<TMessageHandler>(string route)
            where TMessageHandler : IMessageHandler;

        /// <summary>
        /// Adds message handler for a given route to the schema builder.
        /// </summary>
        /// <param name="route">Route to add message handler to.</param>
        /// <typeparam name="TMessage">Type of a payload that comes with message.</typeparam>
        /// <typeparam name="TMessageHandler">Type of a message handler to use for this route.</typeparam>
        /// <returns>Configured builder instance.</returns>
        TBuilder Route<TMessage, TMessageHandler>(string route)
            where TMessageHandler : IMessageHandler<TMessage>;

        /// <summary>
        /// Adds message handler for a given route to the schema builder.
        /// </summary>
        /// <param name="route">Route to add message handler to.</param>
        /// <param name="handlerDelegate">Delegate that handles message.</param>
        /// <returns>Configured builder instance.</returns>
        TBuilder Route(string route, Action<ConnectionContext> handlerDelegate);

        /// <summary>
        /// Adds message handler for a given route to the schema builder.
        /// </summary>
        /// <param name="route">Route to add message handler to.</param>
        /// <param name="handlerDelegate">Delegate that handles message.</param>
        /// <typeparam name="TMessage">Type of a payload that comes with message.</typeparam>
        /// <returns>Configured builder instance.</returns>
        TBuilder Route<TMessage>(string route, Action<ConnectionContext, TMessage> handlerDelegate);

        /// <summary>
        /// Adds hub (series of routes) to the schema builder.
        /// </summary>
        /// <param name="hubRoute">Route to the hub.</param>
        /// <param name="hubConfiguration">Delegate that describes inner routes and hubs.</param>
        /// <returns>Configured builder instance.</returns>
        TBuilder Hub(string hubRoute, Func<SchemaHubBuilder, SchemaHubBuilder> hubConfiguration);
    }
}