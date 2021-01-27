using Socketize.Core.Dto;
using ZeroFormatter;

namespace ConsoleChat.Contract
{
    [ZeroFormattable]
    public class UserLeftDto : WrapperDto<string>
    {
    }
}