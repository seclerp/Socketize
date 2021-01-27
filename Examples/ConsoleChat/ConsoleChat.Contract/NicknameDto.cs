using Socketize.Core.Dto;
using ZeroFormatter;

namespace ConsoleChat.Contract
{
    /// <summary>
    /// Type that represents nickname DTO.
    /// </summary>
    [ZeroFormattable]
    public class NicknameDto : WrapperDto<string>
    {
    }
}