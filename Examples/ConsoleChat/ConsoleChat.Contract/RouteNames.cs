namespace ConsoleChat.Contract
{
    /// <summary>
    /// Class that contains constants for known message route names.
    /// </summary>
    public static class RouteNames
    {
        /// <summary>
        /// Client sends message to a server.
        /// </summary>
        public const string SendMessage = "send-message";

        /// <summary>
        /// Server notifies clients about new message received.
        /// </summary>
        public const string NewMessage = "new-message";

        /// <summary>
        /// Client sends information about nickname preference.
        /// </summary>
        public const string Nickname = "nickname";

        /// <summary>
        /// Server sends current chat state to a newly connected client.
        /// </summary>
        public const string SyncState = "sync-state";
    }
}