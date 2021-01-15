namespace Socketize.Core.Abstractions
{
    /// <summary>
    /// Abstraction that handles messages with payload object of type TMessage.
    /// </summary>
    /// <typeparam name="TMessage">Message object type.</typeparam>
    public interface IMessageHandler<TMessage>
    {
        /// <summary>
        /// Handles message with payload with given context.
        /// </summary>
        /// <param name="connectionContext">Context object containing information about current connection.</param>
        /// <param name="message">Payload message object.</param>
        void Handle(ConnectionContext connectionContext, TMessage message);
    }

    /// <summary>
    /// Abstraction that handles messages.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles message with given context.
        /// </summary>
        /// <param name="connectionContext">Context object containing information about current connection.</param>
        void Handle(ConnectionContext connectionContext);
    }
}