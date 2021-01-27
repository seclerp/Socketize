using System.Collections.Concurrent;

namespace ConsoleChat.Server
{
    public class Message
    {
        public string Nickname { get; set; }

        public string Content { get; set; }
    }

    public class ChatState
    {
        public ConcurrentBag<Message> Messages { get; set; }
    }
}