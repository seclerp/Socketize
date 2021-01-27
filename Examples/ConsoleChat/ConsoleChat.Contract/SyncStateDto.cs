using System.Collections.Generic;
using ZeroFormatter;

namespace ConsoleChat.Contract
{
    /// <summary>
    /// Type that represents sync state DTO.
    /// </summary>
    [ZeroFormattable]
    public class SyncStateDto
    {
        /// <summary>
        /// Gets or sets chat messages collection.
        /// </summary>
        [Index(0)]
        public virtual IEnumerable<MessageStateDto> Messages { get; set; }
    }
}