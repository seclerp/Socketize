using Lidgren.Network;
using Socketize.Core.Abstractions;

namespace Socketize.Core.Services.Abstractions
{
    /// <summary>
    /// Abstraction for service that processes incoming messages.
    /// </summary>
    public interface IProcessingService
    {
        /// <summary>
        /// Processes incoming message to a specific route.
        /// </summary>
        /// <param name="currentPeer">Peer object that represents current peer.</param>
        /// <param name="route">Route path.</param>
        /// <param name="message">Object that represents incoming message.</param>
        /// <param name="failWhenNoHandlers">If true, method throws exception if no matching handlers found.</param>
        /// <param name="ignoreContents">If true, content of this message will be ignored and not deserialized. Useful for ignoring custom layout of Lidgren internal messages.</param>
        void ProcessMessage(IPeer currentPeer, string route, NetIncomingMessage message, bool failWhenNoHandlers = true, bool ignoreContents = false);
    }
}