using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Socketize.Core;
using Socketize.Core.Serialization.Abstractions;
using Socketize.Core.Services.Abstractions;
using Socketize.Server.Configuration;

namespace Socketize.Server
{
    /// <summary>
    /// Peer implementation representing server abstraction that could accept connections from a ClientPeer.
    /// </summary>
    public class ServerPeer : Peer
    {
        private readonly ServerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerPeer"/> class.
        /// </summary>
        /// <param name="processingService">Instance of <see cref="IProcessingService"/> used to process incoming messages.</param>
        /// <param name="logger">Instance of <see cref="ILogger{TCategoryName}"/>.</param>
        /// <param name="options">Server configuration options.</param>
        public ServerPeer(
            IProcessingService processingService,
            IDtoSerializer serializer,
            ILogger<ServerPeer> logger,
            ServerOptions options)
            : base(processingService, serializer, logger)
        {
            _options = options;
        }

        /// <summary>
        /// Returns newly created low level NetPeer object, that is ready to be started.
        /// </summary>
        /// <returns>Newly created low level NetPeer object, that is ready to be started.</returns>
        protected override NetPeer CreateLowLevelPeer()
        {
            var config = new NetPeerConfiguration(_options.AppId)
            {
                Port = _options.Port,
            };
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.AcceptIncomingConnections = true;

            return new NetPeer(config);
        }
    }
}