using MessagePack;

namespace ConsoleChat.Contract
{
    /// <summary>
    /// Type that represents send message DTO.
    /// </summary>
    [MessagePackObject]
    public class SendMessageDto : WrapperDto<string>
    {
    }
}