using ZeroFormatter;

namespace Socketize.Core.Dto
{
    public class WrapperDto<TValue>
    {
        [Index(0)]
        public virtual TValue Value { get; set; }
    }
}