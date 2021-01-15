using Lidgren.Network;

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
        /// <param name="route">Route path.</param>
        /// <param name="message">Object that represents incoming message.</param>
        /// <param name="failWhenNoHandlers">If true, method throws exception if no matching handlers found.</param>
        void ProcessMessage(string route, NetIncomingMessage message, bool failWhenNoHandlers = true);
    }
}