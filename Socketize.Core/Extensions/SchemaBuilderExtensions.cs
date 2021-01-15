using Socketize.Core.Abstractions;
using Socketize.Core.Routing;

namespace Socketize.Core.Extensions
{
    /// <summary>
    /// Extensions for <see cref="SchemaBuilder"/> type.
    /// </summary>
    public static class SchemaBuilderExtensions
    {
        /// <summary>
        /// Adds message handler for Connected event, that fires when new peer is connected to this peer.
        /// </summary>
        /// <param name="builder">Schema builder instance.</param>
        /// <typeparam name="TMessageHandler">Type of message handler.</typeparam>
        /// <returns>Configured schema builder instance.</returns>
        public static SchemaBuilder OnConnect<TMessageHandler>(this SchemaBuilder builder)
            where TMessageHandler : IMessageHandler
        {
            builder.Route<TMessageHandler>(SpecialRouteNames.ConnectRoute);

            return builder;
        }

        /// <summary>
        /// Adds message handler for Disconnected event, that fires when connected peer is disconnected from this peer.
        /// </summary>
        /// <param name="builder">Schema builder instance.</param>
        /// <typeparam name="TMessageHandler">Type of message handler.</typeparam>
        /// <returns>Configured schema builder instance.</returns>
        public static SchemaBuilder OnDisconnect<TMessageHandler>(this SchemaBuilder builder)
            where TMessageHandler : IMessageHandler
        {
            builder.Route<TMessageHandler>(SpecialRouteNames.DisconnectRoute);

            return builder;
        }
    }
}