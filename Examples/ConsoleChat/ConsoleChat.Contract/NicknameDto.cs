using MessagePack;

namespace ConsoleChat.Contract
{
    /// <summary>
    /// Type that represents nickname DTO.
    /// </summary>
    [MessagePackObject]
    public class NicknameDto : WrapperDto<string>
    {
    }
}