using Socketize.Core.Configuration;

namespace Socketize.Server.Configuration
{
    /// <summary>
    /// Options to configure <see cref="ServerPeer"/>.
    /// </summary>
    public class ServerOptions : Options
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerOptions"/> class.
        /// </summary>
        /// <param name="port">Port used to bind server socket.</param>
        /// <param name="appId">Unique identifier across all peers inside one infrastructure. Used in handshake process.</param>
        public ServerOptions(int port, string appId)
            : base(appId)
        {
            Port = port;
        }

        /// <summary>
        /// Gets port used to bind server socket.
        /// </summary>
        public int Port { get; }
    }
}