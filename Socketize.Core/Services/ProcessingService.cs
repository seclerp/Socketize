using Lidgren.Network;
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
        public void ProcessMessage(string route, NetIncomingMessage message, bool failWhenNoHandlers = true)
        {
            var messageLength = message.ReadInt32();
            var dtoRaw = messageLength is 0 ? null : message.ReadBytes(messageLength);
            var context = new ConnectionContext(message.SenderConnection);

            if (!TryProcessMessage(route, context, dtoRaw) && failWhenNoHandlers)
            {
                throw new SocketizeException($"Handler for route '{route}' not found");
            }
        }

        private bool TryProcessMessage(string route, ConnectionContext connectionContext, byte[] dtoRaw)
        {
            if (_messageHandlersManager.RouteExists(route))
            {
                _messageHandlersManager.Invoke(route, connectionContext, dtoRaw);
            }

            return true;
        }
    }
}