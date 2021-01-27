using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Socketize.Client.Configuration;
using Socketize.Core;
using Socketize.Core.Services.Abstractions;

namespace Socketize.Client
{
    /// <summary>
    /// Peer implementation representing client abstraction that could connect to a ServerPeer.
    /// </summary>
    public class ClientPeer : Peer
    {
        private readonly ClientOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPeer"/> class.
        /// </summary>
        /// <param name="processingService">Instance of <see cref="IProcessingService"/> used to process incoming messages.</param>
        /// <param name="logger">Instance of <see cref="ILogger{TCategoryName}"/>.</param>
        /// <param name="options">Client configuration options.</param>
        public ClientPeer(
            IProcessingService processingService,
            ILogger<ClientPeer> logger,
            ClientOptions options)
            : base(processingService, logger)
        {
            _options = options;
        }

        public ConnectionContext ServerContext { get; private set; }

        /// <inheritdoc />
        public override void Start()
        {
            base.Start();

            var approval = LowLevelPeer.CreateMessage();
            approval.Write("Approve me please, there might be token");

            var serverConnection = LowLevelPeer.Connect(_options.ServerHost, _options.ServerPort, approval);
            ServerContext = new ConnectionContext(this, serverConnection);

            Logger.LogInformation($"Send connection approval to {_options.ServerHost}:{_options.ServerPort}");
        }

        /// <inheritdoc />
        protected override NetPeer CreateLowLevelPeer()
        {
            var clientPeerConfiguration = new NetPeerConfiguration(_options.AppId)
            {
                AcceptIncomingConnections = true,
            };

            var clientPeer = new NetClient(clientPeerConfiguration);

            return clientPeer;
        }
    }
}