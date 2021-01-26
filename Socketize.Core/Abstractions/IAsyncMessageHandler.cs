using System.Threading.Tasks;

namespace Socketize.Core.Abstractions
{
    /// <summary>
    /// Abstraction that asynchronously handles messages with payload object of type TMessage.
    /// </summary>
    /// <typeparam name="TMessage">Message object type.</typeparam>
    public interface IAsyncMessageHandler<TMessage>
    {
        /// <summary>
        /// Handles message with payload with given context.
        /// </summary>
        /// <param name="connectionContext">Context object containing information about current connection.</param>
        /// <param name="message">Payload message object.</param>
        /// <returns>Task that represents execution of the handler.</returns>
        Task Handle(ConnectionContext connectionContext, TMessage message);
    }

    /// <summary>
    /// Abstraction that asynchronously handles messages.
    /// </summary>
    public interface IAsyncMessageHandler
    {
        /// <summary>
        /// Handles message with given context.
        /// </summary>
        /// <param name="connectionContext">Context object containing information about current connection.</param>
        /// <returns>Task that represents execution of the handler.</returns>
        Task Handle(ConnectionContext connectionContext);
    }
}