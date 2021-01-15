using Socketize.Core.Configuration;

namespace Socketize.Client.Configuration
{
    /// <summary>
    /// Options to configure <see cref="ClientPeer"/>.
    /// </summary>
    public class ClientOptions : Options
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientOptions"/> class.
        /// </summary>
        /// <param name="serverHost">Server host used to connect.</param>
        /// <param name="serverPort">Server host port used to connect.</param>
        /// <param name="appId">Unique identifier across all peers inside one infrastructure. Used in handshake process.</param>
        public ClientOptions(string serverHost, int serverPort, string appId)
            : base(appId)
        {
            ServerHost = serverHost;
            ServerPort = serverPort;
        }

        /// <summary>
        /// Gets server host used to connect.
        /// </summary>
        public string ServerHost { get; }

        /// <summary>
        /// Gets server host port used to connect.
        /// </summary>
        public int ServerPort { get; }
    }
}