namespace Socketize.Core.Enums
{
    /// <summary>
    /// Enum that represents message delivery mode.
    /// </summary>
    public enum MessageDeliveryMode
    {
        // Corresponding Lidgren.Network delivery methods descriptions credits:
        // https://www.genericgamedev.com/tutorials/lidgren-network-explaining-netdeliverymethod-and-sequence-channels/

        /// <summary>
        /// No guarantees, except for preventing duplicates.
        /// </summary>
        Unreliable,

        /// <summary>
        /// Late messages will be dropped if newer ones were already received.
        /// </summary>
        UnreliableSequenced,

        /// <summary>
        /// All packages will arrive, but not necessarily in the same order.
        /// </summary>
        ReliableUnordered,

        /// <summary>
        /// All packages will arrive, but late ones will be dropped.
        /// This means that we will always receive the latest message eventually, but may miss older ones.
        /// </summary>
        ReliableSequenced,

        /// <summary>
        /// All packages will arrive, and they will do so in the same order.
        /// Unlike all the other methods, here the library will hold back messages until all previous ones are received, before handing them to us.
        /// </summary>
        ReliableOrdered,
    }
}