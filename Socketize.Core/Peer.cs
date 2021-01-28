using System;
using System.Net;
using System.Threading;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Socketize.Core.Abstractions;
using Socketize.Core.Routing;
using Socketize.Core.Serialization.Abstractions;
using Socketize.Core.Services.Abstractions;

namespace Socketize.Core
{
    /// <summary>
    /// High level abstraction for any peer.
    /// </summary>
    public abstract class Peer : IPeer
    {
        private readonly IProcessingService _processingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Peer"/> class.
        /// </summary>
        /// <param name="processingService">service that processes incoming messages.</param>
        /// <param name="logger">Logger instance.</param>
        protected Peer(IProcessingService processingService, IDtoSerializer serializer, ILogger<Peer> logger)
        {
            _processingService = processingService;
            Serializer = serializer;
            Logger = logger;
        }

        /// <inheritdoc/>
        public NetPeer LowLevelPeer { get; private set; }

        /// <inheritdoc/>
        public IDtoSerializer Serializer { get; }

        /// <summary>
        /// Gets logger instance.
        /// </summary>
        protected ILogger<Peer> Logger { get; }

        /// <inheritdoc />
        public ConnectionContext CreateRemoteContext(IPEndPoint target) =>
            new ConnectionContext(this, LowLevelPeer.GetConnection(target), Serializer);

        /// <inheritdoc />
        public virtual void Start()
        {
            Logger.LogInformation("Peer starting");
            LowLevelPeer = CreateLowLevelPeer();
            LowLevelPeer.RegisterReceivedCallback(ProcessMessage);
            LowLevelPeer.Start();
            Logger.LogInformation("Peer started");
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
            Logger.LogInformation("Peer stopping");
            LowLevelPeer.Shutdown("Peer stopping");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Run shutdown if it was not requested yet to release busy port
            if (LowLevelPeer.Status != NetPeerStatus.ShutdownRequested)
            {
                Stop();
            }
        }

        /// <summary>
        /// Returns low level API object to interact with peer.
        /// </summary>
        /// <returns>Low level API object to interact with peer.</returns>
        protected abstract NetPeer CreateLowLevelPeer();

        private void ProcessMessage(object peer)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    var message = ((NetPeer)peer).ReadMessage();
                    ProcessIncomingMessageType(message);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error while processing message");
                }
            });
        }

        private void ProcessIncomingMessageType(NetIncomingMessage message)
        {
            Logger.LogInformation(
                $"Received message with type '{message.MessageType.ToString()}', connection status: {message.SenderConnection.Status}");

            switch (message.MessageType)
            {
                case NetIncomingMessageType.ConnectionApproval:
                    ProcessConnectionApproval(message);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    ProcessStatusChanged(message);
                    break;
                case NetIncomingMessageType.Data:
                    ProcessData(message);
                    break;
                case NetIncomingMessageType.Error:
                case NetIncomingMessageType.UnconnectedData:
                case NetIncomingMessageType.Receipt:
                case NetIncomingMessageType.DiscoveryRequest:
                case NetIncomingMessageType.DiscoveryResponse:
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.NatIntroductionSuccess:
                case NetIncomingMessageType.ConnectionLatencyUpdated:
                    // TODO: Handle
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message.MessageType));
            }
        }

        private void ProcessConnectionApproval(NetIncomingMessage netIncomingMessage)
        {
            // TODO: Validate
            netIncomingMessage.SenderConnection.Approve();
            Logger.LogInformation($"Approved connection for endpoint {netIncomingMessage.SenderEndPoint}");
        }

        private void ProcessStatusChanged(NetIncomingMessage message)
        {
            switch (message.SenderConnection.Status)
            {
                case NetConnectionStatus.Connected:
                    ProcessConnected(message);
                    return;
                case NetConnectionStatus.Disconnected:
                    ProcessDisconnected(message);
                    return;
            }
        }

        private void ProcessConnected(NetIncomingMessage message)
        {
            _processingService.ProcessMessage(this, SpecialRouteNames.ConnectRoute, message, false, true);
            Logger.LogInformation($"'{message.SenderConnection.RemoteEndPoint}' connected");
        }

        private void ProcessDisconnected(NetIncomingMessage message)
        {
            _processingService.ProcessMessage(this, SpecialRouteNames.DisconnectRoute, message, false, true);
            Logger.LogInformation($"'{message.SenderConnection.RemoteEndPoint}' disconnected");
        }

        private void ProcessData(NetIncomingMessage message)
        {
            var route = message.ReadString();
            _processingService.ProcessMessage(this, route, message);
            Logger.LogInformation($"Processed message for route '{route}'");
        }
    }
}