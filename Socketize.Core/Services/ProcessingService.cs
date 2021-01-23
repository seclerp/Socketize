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
        private readonly IMessageHandlersStorage _messageHandlersStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingService"/> class.
        /// </summary>
        public ProcessingService(IMessageHandlersStorage messageHandlersStorage)
        {
            _messageHandlersStorage = messageHandlersStorage;
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
            if (_messageHandlersStorage.HasRoute(route))
            {
                _messageHandlersStorage.Invoke(route, connectionContext, dtoRaw);
            }

            return true;
        }
    }
}