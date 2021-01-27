using System.Collections.Concurrent;

namespace ConsoleChat.Server
{
    /// <summary>
    /// Type that represents chat state.
    /// </summary>
    public class ChatState
    {
        /// <summary>
        /// Gets or sets orderer collection of messages.
        /// </summary>
        public ConcurrentQueue<Message> Messages { get; set; }
    }
}