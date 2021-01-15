using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;

namespace Socketize.Core
{
    /// <summary>
    /// Object representing current connection context.
    /// </summary>
    public class ConnectionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionContext"/> class.
        /// </summary>
        /// <param name="connection">Remote paired connection object, representing connection between client and server.</param>
        public ConnectionContext(NetConnection connection)
        {
            Connection = connection;
        }

        /// <summary>
        /// Gets remote paired connection object, representing connection between client and server.
        /// On server, Peer property is server low level peer object.
        /// On client, Peer property is client low level peer object.
        /// </summary>
        public NetConnection Connection { get; }

        /// <summary>
        /// Gets all connections currently connected to peer.
        /// </summary>
        public IEnumerable<NetConnection> All => Connection.Peer.Connections;

        /// <summary>
        /// Gets all connections currently connected to peer, except current remote connection.
        /// </summary>
        public IEnumerable<NetConnection> Others => Connection.Peer.Connections.Where(conn => conn != Connection);
    }
}