using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Socketize.Abstractions;

namespace Socketize.Extensions.DependencyInjection
{
  public class ServerHostedService : IHostedService
  {
    private readonly IPeer _peerService;

    public ServerHostedService(IPeer peerService)
    {
      _peerService = peerService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _peerService.Start();

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _peerService.Stop();

      return Task.CompletedTask;
    }
  }
}