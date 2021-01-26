using System;
using Socketize.Core.Abstractions;

namespace Socketize.Core.Routing.Abstractions
{
    /// <summary>
    /// Base abstractions for any route builder type.
    /// </summary>
    /// <typeparam name="TBuilder">Type of a scheme builder that implements this abstraction.</typeparam>
    public interface IRoutesBuilder<TBuilder>
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
    }
}