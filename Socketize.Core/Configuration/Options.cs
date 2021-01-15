namespace Socketize.Core.Configuration
{
    /// <summary>
    /// Base class for <see cref="Peer"/> configuration.
    /// </summary>
    public abstract class Options
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        /// <param name="appId">Unique identifier across all peers inside one infrastructure. Used in handshake process.</param>
        protected Options(string appId)
        {
            AppId = appId;
        }

        /// <summary>
        /// Gets unique identifier across all peers inside one infrastructure.
        /// Used in handshake process.
        /// </summary>
        public string AppId { get; }
    }
}