using MessagePack;

namespace ConsoleChat.Contract
{
    /// <summary>
    /// Type that represents new message DTO.
    /// </summary>
    [MessagePackObject]
    public class NewMessageDto
    {
        /// <summary>
        /// Gets or sets nickname of a user.
        /// </summary>
        [Key(0)]
        public virtual string Nickname { get; set; }

        /// <summary>
        /// Gets or sets user message content.
        /// </summary>
        [Key(1)]
        public virtual string Content { get; set; }
    }
}