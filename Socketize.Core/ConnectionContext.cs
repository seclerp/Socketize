using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Socketize.Core.Abstractions;
using Socketize.Core.Serialization.Abstractions;

namespace Socketize.Core
{
    /// <summary>
    /// Object representing current connection context.
    /// </summary>
    public class ConnectionContext
    {
        private readonly IDtoSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionContext"/> class.
        /// </summary>
        /// <param name="currentPeer">Peer object that represents current peer.</param>
        /// <param name="connection">Remote paired connection object, representing connection between client and server.</param>
        /// <param name="serializer">DTO serializer instance.</param>
        public ConnectionContext(IPeer currentPeer, NetConnection connection, IDtoSerializer serializer)
        {
            _serializer = serializer;
            CurrentPeer = currentPeer;
            Connection = connection;
        }

        /// <summary>
        /// Gets peer object that represents current peer.
        /// </summary>
        public IPeer CurrentPeer { get; }

        /// <summary>
        /// Gets remote paired connection object, representing connection between client and server.
        /// On server, Peer property is server low level peer object.
        /// On client, Peer property is client low level peer object.
        /// </summary>
        public NetConnection Connection { get; }

        /// <summary>
        /// Gets all connections currently connected to peer.
        /// </summary>
        public IEnumerable<NetConnection> All => CurrentPeer.LowLevelPeer.Connections;

        /// <summary>
        /// Gets all connections currently connected to peer, except current remote connection.
        /// </summary>
        public IEnumerable<NetConnection> Others => CurrentPeer.LowLevelPeer.Connections.Where(conn => conn != Connection);

        /// <summary>
        /// Prepares new outgoing low level message.
        /// </summary>
        /// <param name="route">Route to send message to.</param>
        /// <param name="messageDto">Message DTO payload object.</param>
        /// <typeparam name="T">Type of message DTO payload object.</typeparam>
        /// <returns>Outgoing low level message.</returns>
        public NetOutgoingMessage CreateMessage<T>(string route, T messageDto)
        {
            var dtoRaw = _serializer.Serialize(messageDto);
            var message = CurrentPeer.LowLevelPeer.CreateMessage();
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