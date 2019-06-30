using Microsoft.Extensions.DependencyInjection;

namespace Socketize.Extensions.AspNetCore.Hosting
{
  public static class HostingExtensions
  {
    public static IServiceCollection AddSocketizeHosting(this IServiceCollection services)
    {
      services.AddHostedService<ServerHostedService>();

      return services;
    }
  }
}