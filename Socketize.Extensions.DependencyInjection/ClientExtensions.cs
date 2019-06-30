using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Socketize.Abstractions;
using Socketize.Routing;

namespace Socketize.Extensions.DependencyInjection
{
  public static class ClientExtensions
  {
    public static IServiceCollection AddSocketizeClient<TRouteMap>(this IServiceCollection services, ClientOptions options) where TRouteMap : IRouteMap
    {
      services.AddSingleton(CreateSchema<TRouteMap>);
      AddSocketizeClientInternal(services, options);

      return services;
    }

    public static IServiceCollection AddSocketizeClient<TRouteMap>(this IServiceCollection services, ClientOptions options, ILogger<Client> logger) where TRouteMap : IRouteMap
    {
      services.AddSingleton(logger);

      return services.AddSocketizeClient<TRouteMap>(options);
    }

    public static IServiceCollection AddSocketizeClient(this IServiceCollection services, Func<SchemaBuilder, SchemaBuilder> builderAction, ClientOptions options)
    {
      services.AddSingleton(serviceProvider => CreateSchema(builderAction));
      AddSocketizeClientInternal(services, options);

      return services;
    }

    public static IServiceCollection AddSocketizeClient(this IServiceCollection services, Func<SchemaBuilder, SchemaBuilder> builderAction, ClientOptions options, ILogger<Client> logger)
    {
      services.AddSingleton(logger);

      return services.AddSocketizeClient(builderAction, options);
    }

    private static void AddSocketizeClientInternal(IServiceCollection services, ClientOptions options)
    {
      RegisterCommonServices(services, options);
      RegisterHandlers(services.BuildServiceProvider().GetService<Schema>(), services);
    }

    private static void RegisterCommonServices(this IServiceCollection services, ClientOptions options)
    {
      services.AddSingleton<IMessageHandlerFactory>(serviceProvider => new ServicesHandlerFactory(serviceProvider));
      services.AddTransient<IProcessingService, ProcessingService>();
      services.AddSingleton<IPeer, Client>(serviceProvider => new Client(
        serviceProvider.GetService<IProcessingService>(),
        serviceProvider.GetService<ILogger<Client>>(),
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