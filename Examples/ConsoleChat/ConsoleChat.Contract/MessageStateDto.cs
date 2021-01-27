using ZeroFormatter;

namespace ConsoleChat.Contract
{
    /// <summary>
    /// Type that represents message state DTO.
    /// </summary>
    [ZeroFormattable]
    public class MessageStateDto
    {
        /// <summary>
        /// Gets or sets nickname of a user.
        /// </summary>
        [Index(0)]
        public virtual string Nickname { get; set; }

        /// <summary>
        /// Gets or sets user message content.
        /// </summary>
        [Index(1)]
        public virtual string Content { get; set; }
    }
}