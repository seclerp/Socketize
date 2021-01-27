using System.Threading.Tasks;
using Lidgren.Network;
using Socketize.Core.Abstractions;
using Socketize.Core.Exceptions;
using Socketize.Core.Services.Abstractions;

namespace Socketize.Core.Services
{
    /// <summary>
    /// Service that process incoming messages.
    /// </summary>
    public class ProcessingService : IProcessingService
    {
        private readonly IMessageHandlersManager _messageHandlersManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingService"/> class.
        /// </summary>
        /// <param name="messageHandlersManager">Manager for message handlers.</param>
        public ProcessingService(IMessageHandlersManager messageHandlersManager)
        {
            _messageHandlersManager = messageHandlersManager;
        }

        /// <inheritdoc />
        public void ProcessMessage(IPeer currentPeer, string route, NetIncomingMessage message, bool failWhenNoHandlers = true, bool ignoreContents = false)
        {
            byte[] dtoRaw = null;
            if (!ignoreContents)
            {
                var messageLength = message.ReadInt32();
                dtoRaw = messageLength is 0 ? null : message.ReadBytes(messageLength);
            }

            var context = new ConnectionContext(currentPeer, message.SenderConnection);

            if (!TryQueueMessageProcessing(route, context, dtoRaw) && failWhenNoHandlers)
            {
                throw new SocketizeException($"Handler for route '{route}' not found");
            }
        }

        private bool TryQueueMessageProcessing(string route, ConnectionContext connectionContext, byte[] dtoRaw)
        {
            if (_messageHandlersManager.RouteExists(route))
            {
                Task.Run(() => _messageHandlersManager.Invoke(route, connectionContext, dtoRaw));
            }

            return true;
        }
    }
}