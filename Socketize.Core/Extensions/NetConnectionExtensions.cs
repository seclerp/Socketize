using System;
using Lidgren.Network;

namespace Socketize.Core.Extensions
{
    /// <summary>
    /// Extensions for <see cref="NetConnection"/> type.
    /// </summary>
    public static class NetConnectionExtensions
    {
        /// <summary>
        /// Waits when NetConnection will be ready for sending messages.
        /// </summary>
        /// <param name="connection">Instance of <see cref="NetConnection"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">When status is out or range of the known ones.</exception>
        public static void WaitForReadiness(this NetConnection connection)
        {
            while (true)
            {
                switch (connection.Status)
                {
                    case NetConnectionStatus.Disconnected:
                    case NetConnectionStatus.Disconnecting:
                    case NetConnectionStatus.Connected:
                    case NetConnectionStatus.ReceivedInitiation:
                        return;

                    case NetConnectionStatus.None:
                    case NetConnectionStatus.InitiatedConnect:
                    case NetConnectionStatus.RespondedAwaitingApproval:
                    case NetConnectionStatus.RespondedConnect:
                        continue;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}