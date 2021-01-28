using System.Collections.Generic;
using MessagePack;

namespace ConsoleChat.Contract
{
    /// <summary>
    /// Type that represents sync state DTO.
    /// </summary>
    [MessagePackObject]
    public class SyncStateDto
    {
        /// <summary>
        /// Gets or sets chat messages collection.
        /// </summary>
        [Key(0)]
        public virtual IEnumerable<MessageStateDto> Messages { get; set; }
    }
}