using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lidgren.Network;
using Socketize.Core.Enums;

namespace Socketize.Core.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ConnectionContext"/> type.
    /// </summary>
    public static class ConnectionContextExtensions
    {
        /// <summary>
        /// Returns connection object to a connected remote endpoint.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <param name="endpoint">Remote endpoint.</param>
        /// <returns>Connection object to a connected remote endpoint.</returns>
        public static NetConnection GetConnection(this ConnectionContext connectionContext, IPEndPoint endpoint)
        {
            return connectionContext.CurrentPeer.LowLevelPeer.GetConnection(endpoint);
        }

        /// <summary>
        /// Sends message DTO to a current connected remote peer using specific route.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <param name="route">Route path.</param>
        /// <param name="messageDto">Object representing message data.</param>
        /// <param name="deliveryMode">Message delivery mode, ReliableOrdered by default.</param>
        /// <typeparam name="T">Type of message data.</typeparam>
        public static void Send<T>(
            this ConnectionContext connectionContext,
            string route,
            T messageDto,
            MessageDeliveryMode deliveryMode = MessageDeliveryMode.ReliableOrdered)
        {
            connectionContext.SendInternal(route, messageDto, connectionContext.Connection, deliveryMode);
        }

        /// <summary>
        /// Sends message DTO to a other connected remote endpoint using specific route.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <param name="endpoint">Connected remote endpoint.</param>
        /// <param name="route">Route path.</param>
        /// <param name="messageDto">Object representing message data.</param>
        /// <param name="deliveryMode">Message delivery mode, ReliableOrdered by default.</param>
        /// <typeparam name="T">Type of message data.</typeparam>
        public static void SendTo<T>(
            this ConnectionContext connectionContext,
            IPEndPoint endpoint,
            string route,
            T messageDto,
            MessageDeliveryMode deliveryMode = MessageDeliveryMode.ReliableOrdered)
        {
            connectionContext.SendInternal(route, messageDto, connectionContext.GetConnection(endpoint), deliveryMode);
        }

        /// <summary>
        /// Sends message DTO to all currently connected remote peers using specific route.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <param name="route">Route path.</param>
        /// <param name="messageDto">Object representing message data.</param>
        /// <param name="deliveryMode">Message delivery mode, ReliableOrdered by default.</param>
        /// <typeparam name="T">Type of message data.</typeparam>
        public static void SendToAll<T>(
            this ConnectionContext connectionContext,
            string route,
            T messageDto,
            MessageDeliveryMode deliveryMode = MessageDeliveryMode.ReliableOrdered)
        {
            connectionContext.SendInternal(route, messageDto, connectionContext.All, deliveryMode);
        }

        /// <summary>
        /// Sends message DTO to all currently connected remote peers, except current remote connected peer, using specific route.
        /// </summary>
        /// <param name="connectionContext">Instance of <see cref="ConnectionContext"/>.</param>
        /// <param name="route">Route path.</param>
        /// <param name="messageDto">Object representing message data.</param>
        /// <param name="deliveryMode">Message delivery mode, ReliableOrdered by default.</param>
        /// <typeparam name="T">Type of message data.</typeparam>
        public static void SendToOthers<T>(
            this ConnectionContext connectionContext,
            string route,
            T messageDto,
            MessageDeliveryMode deliveryMode = MessageDeliveryMode.ReliableOrdered)
        {
            connectionContext.SendInternal(route, messageDto, connectionContext.Others, deliveryMode);
        }

        private static void SendInternal<T>(
            this ConnectionContext connectionContext,
            string route,
            T messageDto,
            IEnumerable<NetConnection> connections,
            MessageDeliveryMode deliveryMode)
        {
            Parallel.ForEach(connections, connection => connectionContext.SendInternal(route, messageDto, connection, deliveryMode));
        }

        private static void SendInternal<T>(this ConnectionContext connectionContext, string route, T messageDto, NetConnection connection, MessageDeliveryMode deliveryMode)
        {
            var message = connectionContext.CreateMessage(route, messageDto);
            var deliveryMethod = Map(deliveryMode);

            // TODO: Make sequence channel configurable
            // https://github.com/seclerp/Socketize/issues/12
            connection.SendMessage(message, deliveryMethod, 0);
        }

        private static NetDeliveryMethod Map(MessageDeliveryMode deliveryMode)
        {
            return deliveryMode switch
            {
                MessageDeliveryMode.Unreliable => NetDeliveryMethod.Unreliable,
                MessageDeliveryMode.UnreliableSequenced => NetDeliveryMethod.UnreliableSequenced,
                MessageDeliveryMode.ReliableUnordered => NetDeliveryMethod.ReliableUnordered,
                MessageDeliveryMode.ReliableSequenced => NetDeliveryMethod.ReliableSequenced,
                MessageDeliveryMode.ReliableOrdered => NetDeliveryMethod.ReliableOrdered,
                _ => throw new ArgumentOutOfRangeException(nameof(deliveryMode), deliveryMode, null),
            };
        }
    }
}