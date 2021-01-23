namespace Socketize.Core.Enums
{
    /// <summary>
    /// Kind of message handler.
    /// </summary>
    public enum HandlerInstanceKind
    {
        /// <summary>
        /// Message handler represented as class instance.
        /// </summary>
        Class,

        /// <summary>
        /// Message handler represented as delegate or lambda.
        /// </summary>
        Delegate,
    }
}