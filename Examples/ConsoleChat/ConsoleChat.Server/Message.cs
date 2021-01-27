namespace ConsoleChat.Server
{
    /// <summary>
    /// Type that represents message model.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets user's nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets message content.
        /// </summary>
        public string Content { get; set; }
    }
}