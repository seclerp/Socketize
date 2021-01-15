using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lidgren.Network;
using ZeroFormatter;

namespace Socketize.Core.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ConnectionContext"/> type.
    /// </summary>
    public static class ConnectionContextExtensions
    {
        /// <summary>
        /// Returns new empty message to populate and send to a remote peer.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <returns>Message object ready to be populated and to be sent.</returns>
        public static NetOutgoingMessage NewMessage(this ConnectionContext connectionContext)
        {
            return connectionContext.Connection.Peer.CreateMessage();
        }

        /// <summary>
        /// Returns connection object to a connected remote endpoint.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <param name="endpoint">Remote endpoint.</param>
        /// <returns>Connection object to a connected remote endpoint.</returns>
        public static NetConnection GetConnection(this ConnectionContext connectionContext, IPEndPoint endpoint)
        {
            return connectionContext.Connection.Peer.GetConnection(endpoint);
        }

        /// <summary>
        /// Sends message DTO to a current connected remote peer using specific route.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <param name="route">Route path.</param>
        /// <param name="messageDto">Object representing message data.</param>
        /// <typeparam name="T">Type of message data.</typeparam>
        public static void Send<T>(this ConnectionContext connectionContext, string route, T messageDto)
        {
            connectionContext.SendInternal(route, messageDto, connectionContext.Connection);
        }

        /// <summary>
        /// Sends message DTO to a other connected remote endpoint using specific route.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <param name="endpoint">Connected remote endpoint.</param>
        /// <param name="route">Route path.</param>
        /// <param name="messageDto">Object representing message data.</param>
        /// <typeparam name="T">Type of message data.</typeparam>
        public static void SendTo<T>(this ConnectionContext connectionContext, IPEndPoint endpoint, string route, T messageDto)
        {
            connectionContext.SendInternal(route, messageDto, connectionContext.GetConnection(endpoint));
        }

        /// <summary>
        /// Sends message DTO to all currently connected remote peers using specific route.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <param name="route">Route path.</param>
        /// <param name="messageDto">Object representing message data.</param>
        /// <typeparam name="T">Type of message data.</typeparam>
        public static void SendToAll<T>(this ConnectionContext connectionContext, string route, T messageDto)
        {
            connectionContext.SendInternal(route, messageDto, connectionContext.All);
        }

        /// <summary>
        /// Sends message DTO to all currently connected remote peers, except current remote connected peer, using specific route.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <param name="route">Route path.</param>
        /// <param name="messageDto">Object representing message data.</param>
        /// <typeparam name="T">Type of message data.</typeparam>
        public static void SendToOthers<T>(this ConnectionContext connectionContext, string route, T messageDto)
        {
            connectionContext.SendInternal(route, messageDto, connectionContext.Others);
        }

        private static void SendInternal<T>(this ConnectionContext connectionContext, string route, T messageDto, IEnumerable<NetConnection> connections)
        {
            Parallel.ForEach(connections, connection => connectionContext.SendInternal(route, messageDto, connection));
        }

        private static void SendInternal<T>(this ConnectionContext connectionContext, string route, T messageDto, NetConnection connection)
        {
            var message = connectionContext.PrepareRawMessage(route, messageDto);

            // TODO: Make delivery method optionally configurable
            connection.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private static NetOutgoingMessage PrepareRawMessage<T>(this ConnectionContext connectionContext, string route, T messageDto)
        {
            var dtoRaw = ZeroFormatterSerializer.Serialize(messageDto);
            var message = connectionContext.NewMessage();
            message.Write(route);
            message.Write(dtoRaw.Length);
            if (dtoRaw.Length != 0)
            {
                message.Write(dtoRaw);
            }

            return message;
        }
    }
}