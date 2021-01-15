using System;

namespace Socketize.Core.Abstractions
{
    /// <summary>
    /// Factory that creates <see cref="IMessageHandler{TMessage}"/> or <see cref="IMessageHandler"/> instances.
    /// </summary>
    public interface IMessageHandlerFactory
    {
        /// <summary>
        /// Creates message handler of statically defined type TMessageHandler.
        /// </summary>
        /// <typeparam name="TMessageHandler">Statically defined type of message handler to create instance.</typeparam>
        /// <returns>Instance of statically defined type of message handler.</returns>
        TMessageHandler Get<TMessageHandler>();

        /// <summary>
        /// Creates message handler of dynamically defined type.
        /// </summary>
        /// <param name="type">Dynamically defined type object of message handler to create instance.</param>
        /// <returns>Instance of dynamically defined type of message handler.</returns>
        object Get(Type type);
    }
}