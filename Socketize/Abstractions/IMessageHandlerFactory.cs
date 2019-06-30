using System;

namespace Socketize.Abstractions
{
  public interface IMessageHandlerFactory
  {
    TMessageHandler Get<TMessageHandler>();
    object Get(Type type);
  }
}