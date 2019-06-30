using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Socketize.Abstractions;
using Socketize.Routing;

namespace Socketize.Extensions.DependencyInjection
{
  public static class ServerExtensions
  {
    public static IServiceCollection AddSocketizeServer<TRouteMap>(this IServiceCollection services, ServerOptions options) where TRouteMap : IRouteMap
    {
      services.AddSingleton(CreateSchema<TRouteMap>);
      AddSocketizeServerInternal(services, options);

      return services;
    }

    public static IServiceCollection AddSocketizeServer(this IServiceCollection services, Func<SchemaBuilder, SchemaBuilder> builderAction, ServerOptions options)
    {
      services.AddSingleton(serviceProvider => CreateSchema(builderAction));
      AddSocketizeServerInternal(services, options);

      return services;
    }

    private static void AddSocketizeServerInternal(IServiceCollection services, ServerOptions options)
    {
      RegisterCommonServices(services, options);
      RegisterHandlers(services.BuildServiceProvider().GetService<Schema>(), services);
    }

    private static void RegisterCommonServices(this IServiceCollection services, ServerOptions options)
    {
      services.AddSingleton<IMessageHandlerFactory>(serviceProvider => new ServicesHandlerFactory(serviceProvider));
      services.AddTransient<IProcessingService, ProcessingService>();
      services.AddSingleton<IPeer, Server>(serviceProvider => new Server(
        serviceProvider.GetService<IProcessingService>(),
        serviceProvider.GetService<ILogger<Server>>(),
        options
      ));
      services.AddHostedService<ServerHostedService>();
    }

    private static Schema CreateSchema<TRouteMap>(IServiceProvider serviceProvider)
    {
      var socketizeRootBuilder = new SchemaBuilder();
      var routeMap = (IRouteMap) Activator.CreateInstance<TRouteMap>();

      return routeMap.Declare(socketizeRootBuilder).BuildSchema();
    }

    private static Schema CreateSchema(Func<SchemaBuilder, SchemaBuilder> builderAction)
    {
      var socketizeRootBuilder = new SchemaBuilder();

      return builderAction(socketizeRootBuilder).BuildSchema();
    }

    private static void RegisterHandlers(Schema schema, IServiceCollection services)
    {
      foreach (var item in schema.Special.Items)
      {
        services.AddTransient(item.HandlerType);
      }

      foreach (var part in schema.Parts)
      {
        foreach (var item in part.Items)
        {
          services.AddTransient(item.HandlerType);
        }
      }
    }
  }
}