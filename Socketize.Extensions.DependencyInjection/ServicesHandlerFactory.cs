using System;
using Microsoft.Extensions.DependencyInjection;
using Socketize.Abstractions;

namespace Socketize.Extensions.DependencyInjection
{
  public class ServicesHandlerFactory : IMessageHandlerFactory
  {
    private readonly IServiceProvider _services;

    public ServicesHandlerFactory(IServiceProvider services)
    {
      _services = services;
    }

    public TMessageHandler Get<TMessageHandler>() => _services.GetService<TMessageHandler>();
    public object Get(Type type) => _services.GetService(type);
  }
}