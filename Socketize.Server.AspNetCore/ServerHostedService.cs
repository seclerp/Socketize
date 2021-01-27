using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using Microsoft.Extensions.Hosting;
using Socketize.Core.Abstractions;

namespace Socketize.Server.AspNetCore
{
    /// <summary>
    /// Hosted service that bootstraps and controls lifetime of Socketize server instance.
    /// </summary>
    public class ServerHostedService : IHostedService
    {
        private readonly IPeer _peer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHostedService"/> class.
        /// </summary>
        /// <param name="peer">Server peer instance to use for accepting connections.</param>
        public ServerHostedService(IPeer peer)
        {
            _peer = peer;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _peer.Start();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _peer.Stop();
            _peer.Dispose();

            return Task.CompletedTask;
        }
    }
}