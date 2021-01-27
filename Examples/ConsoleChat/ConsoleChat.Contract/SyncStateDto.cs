using System.Collections.Generic;
using ZeroFormatter;

namespace ConsoleChat.Contract
{
    [ZeroFormattable]
    public class SyncStateDto
    {
        [Index(0)]
        public virtual IEnumerable<MessageStateDto> Messages { get; set; }
    }
}