using System;
using System.Net;
using Lidgren.Network;
using Socketize.Core.Serialization.Abstractions;

namespace Socketize.Core.Abstractions
{
    /// <summary>
    /// Abstraction for any connected instance.
    /// </summary>
    public interface IPeer : IDisposable
    {
        /// <summary>
        /// Gets object to interact with the connection.
        /// </summary>
        NetPeer LowLevelPeer { get; }

        /// <summary>
        /// Gets serializer instance..
        /// </summary>
        IDtoSerializer Serializer { get; }

        /// <summary>
        /// Starts and configures this <see cref="IPeer"/> instance.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this <see cref="IPeer"/> instance.
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns <see cref="ConnectionContext"/> object representing connection to remote endpoint.
        /// </summary>
        /// <param name="target">Remote endpoint address.</param>
        /// <returns>Object representing connection to remote endpoint.</returns>
        ConnectionContext CreateRemoteContext(IPEndPoint target);
    }
}