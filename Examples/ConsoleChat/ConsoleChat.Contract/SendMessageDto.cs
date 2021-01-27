using Socketize.Core.Dto;
using ZeroFormatter;

namespace ConsoleChat.Contract
{
    /// <summary>
    /// Type that represents send message DTO.
    /// </summary>
    [ZeroFormattable]
    public class SendMessageDto : WrapperDto<string>
    {
    }
}