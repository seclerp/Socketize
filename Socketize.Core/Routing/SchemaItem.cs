using System;
using Socketize.Core.Enums;

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
        /// <param name="handler">Object that represents handler instance.</param>
        /// <param name="messageType">Type that represents message payload.</param>
        /// <param name="kind">Kind of a handler instance.</param>
        public SchemaItem(string route, object handler, Type messageType, HandlerInstanceKind kind)
        {
            Route = route;
            Handler = handler;
            MessageType = messageType;
            Kind = kind;
        }

        /// <summary>
        /// Gets string that represents route to a handler.
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Gets type that represents handler that will handle message.
        /// </summary>
        public object Handler { get; }

        /// <summary>
        /// Gets type that represents message payload.
        /// Returns null if message has no payload.
        /// </summary>
        public Type MessageType { get; }

        /// <summary>
        /// Gets kind of message handler object.
        /// </summary>
        public HandlerInstanceKind Kind { get; }
    }
}