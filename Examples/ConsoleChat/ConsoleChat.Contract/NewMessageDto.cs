using ZeroFormatter;

namespace ConsoleChat.Contract
{
    [ZeroFormattable]
    public class NewMessageDto
    {
        [Index(0)]
        public virtual string Nickname { get; set; }

        [Index(1)]
        public virtual string Content { get; set; }
    }
}