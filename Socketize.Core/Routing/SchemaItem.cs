using System;

namespace Socketize.Core.Routing
{
    /// <summary>
    /// Type that represents item inside schema, containing route and message handler information.
    /// </summary>
    public class SchemaItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaItem"/> class.
        /// </summary>
        /// <param name="route">String that represents route to a handler.</param>
        /// <param name="handlerType">Type that represents handler that will handle message.</param>
        /// <param name="messageType">Type that represents message payload.</param>
        public SchemaItem(string route, Type handlerType, Type messageType)
        {
            Route = route;
            HandlerType = handlerType;
            MessageType = messageType;
        }

        /// <summary>
        /// Gets string that represents route to a handler.
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Gets type that represents handler that will handle message.
        /// </summary>
        public Type HandlerType { get; }

        /// <summary>
        /// Gets type that represents message payload.
        /// Returns null if message has no payload.
        /// </summary>
        public Type MessageType { get; }
    }
}