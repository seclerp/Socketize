using ZeroFormatter;

namespace Socketize.Core.Dto
{
    /// <summary>
    /// Shortcut class for creating simple wrapper DTOs.
    /// Inherit from it to create custom wrapped DTO.
    /// </summary>
    /// <typeparam name="TValue">Type that represents type of wrapped value.</typeparam>
    public class WrapperDto<TValue>
    {
        /// <summary>
        /// Gets or sets value of wrapper object.
        /// </summary>
        [Index(0)]
        public virtual TValue Value { get; set; }
    }
}