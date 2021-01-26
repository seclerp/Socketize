using System;
using System.Threading.Tasks;
using Socketize.Core.Abstractions;

namespace Socketize.Core.Routing.Abstractions
{
    /// <summary>
    /// Base abstractions for any asynchronous route builder type.
    /// </summary>
    /// <typeparam name="TBuilder">Type of a scheme builder that implements this abstraction.</typeparam>
    public interface IAsyncRoutesBuilder<out TBuilder>
    {
        /// <summary>
        /// Adds asynchronous message handler for a given route to the schema builder.
        /// </summary>
        /// <param name="route">Route to add message handler to.</param>
        /// <typeparam name="TMessageHandler">Type of a message handler to use for this route.</typeparam>
        /// <returns>Configured builder instance.</returns>
        TBuilder AsyncRoute<TMessageHandler>(string route)
            where TMessageHandler : IAsyncMessageHandler;

        /// <summary>
        /// Adds asynchronous message handler for a given route to the schema builder.
        /// </summary>
        /// <param name="route">Route to add message handler to.</param>
        /// <typeparam name="TMessage">Type of a payload that comes with message.</typeparam>
        /// <typeparam name="TMessageHandler">Type of a message handler to use for this route.</typeparam>
        /// <returns>Configured builder instance.</returns>
        TBuilder AsyncRoute<TMessage, TMessageHandler>(string route)
            where TMessageHandler : IAsyncMessageHandler<TMessage>;

        /// <summary>
        /// Adds asynchronous message handler for a given route to the schema builder.
        /// </summary>
        /// <param name="route">Route to add message handler to.</param>
        /// <param name="handlerDelegate">Delegate that handles message.</param>
        /// <returns>Configured builder instance.</returns>
        TBuilder AsyncRoute(string route, Func<ConnectionContext, Task> handlerDelegate);

        /// <summary>
        /// Adds asynchronous message handler for a given route to the schema builder.
        /// </summary>
        /// <param name="route">Route to add message handler to.</param>
        /// <param name="handlerDelegate">Delegate that handles message.</param>
        /// <typeparam name="TMessage">Type of a payload that comes with message.</typeparam>
        /// <returns>Configured builder instance.</returns>
        TBuilder AsyncRoute<TMessage>(string route, Func<ConnectionContext, TMessage, Task> handlerDelegate);
    }
}