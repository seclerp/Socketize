using System.Threading.Tasks;

namespace Socketize.Core.Services.Abstractions
{
    /// <summary>
    /// Abstraction that represents manager for message handlers.
    /// </summary>
    public interface IMessageHandlersManager
    {
        /// <summary>
        /// Invokes handler for given route with context and DTO.
        /// </summary>
        /// <param name="route">Route used to determine which handler to use.</param>
        /// <param name="context">Connection context instance.</param>
        /// <param name="dtoRaw">Raw DTO object containing byte array with information, could be null.</param>
        /// <returns>Task representing handler processing.</returns>
        Task Invoke(string route, ConnectionContext context, byte[] dtoRaw);

        /// <summary>
        /// Returns true if handler for given route exists, otherwise false.
        /// </summary>
        /// <param name="route">Route used to determine which handler to use.</param>
        /// <returns>True if handler for given route exists, otherwise false.</returns>
        bool RouteExists(string route);
    }
}