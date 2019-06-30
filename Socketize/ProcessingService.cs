using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lidgren.Network;
using Socketize.Abstractions;
using Socketize.Exceptions;
using Socketize.Routing;
using ZeroFormatter;

namespace Socketize
{
  // Singleton
  public class ProcessingService : IProcessingService
  {
    private readonly IDictionary<string, MethodInfo> _handlersMethodInfo;
    private readonly IDictionary<string, Action<Context, byte[]>> _handlers;
    private readonly IMessageHandlerFactory _factory;

    public ProcessingService(Schema schema, IMessageHandlerFactory factory)
    {
      _factory = factory;
      var allSchemaItems = schema.Special.Items.Union(schema.Parts.SelectMany(part => part.Items)).ToDictionary(item => item.Route, item => item);
      _handlersMethodInfo = CreateMessageHandlersMethodInfo(allSchemaItems);
      _handlers = CreateMessageHandlers(allSchemaItems);
    }

    public void ProcessMessage(string route, NetIncomingMessage message, bool failWhenNoHandlers = true)
    {
      var messageLength = message.ReadInt32();
      var dtoRaw = messageLength is 0 ? null : message.ReadBytes(messageLength);
      var context = new Context(message.SenderConnection);

      if (!TryProcessMessage(route, context, dtoRaw))
      {
        if (failWhenNoHandlers)
        {
          throw new SocketizeException($"Handler for route '{route}' not found");
        }
      }
    }

    public void ProcessConnected(NetConnection connection)
    {
      var context = new Context(connection);
      TryProcessMessage(SpecialRouteNames.ConnectRoute, context, null);
    }

    public void ProcessDisconnected(NetConnection connection)
    {
      var context = new Context(connection);
      TryProcessMessage(SpecialRouteNames.DisconnectRoute, context, null);
    }

    private IDictionary<string, MethodInfo> CreateMessageHandlersMethodInfo(IDictionary<string, SchemaItem> schemaItems) =>
      schemaItems.ToDictionary(kv => kv.Key, kv =>
        kv.Value.HandlerType
          .GetMethods()
          .Where(info => info.Name is "Handle")
          .FirstOrDefault(methodInfo => IsValidHandler(methodInfo, kv.Value.MessageType)));

    private bool IsValidHandler(MethodInfo method, Type messageType)
    {
      var parameters = method.GetParameters();
      if (messageType is null
          && parameters.Length == 1
          && parameters[0].ParameterType.IsAssignableFrom(typeof(Context)))
      {
        return true;
      }

      if (parameters.Length == 2
          && parameters[0].ParameterType.IsAssignableFrom(typeof(Context))
          && parameters[1].ParameterType.IsAssignableFrom(messageType))
      {
        return true;
      }

      return false;
    }

    private IDictionary<string, Action<Context, byte[]>> CreateMessageHandlers(IDictionary<string, SchemaItem> schemaItems)
    {
      return schemaItems.ToDictionary(kv => kv.Key, kv =>
        new Action<Context, byte[]>((context, dtoRaw) =>
        {
          var instance = _factory.Get(kv.Value.HandlerType);

          object[] args;
          if (kv.Value.MessageType is null)
          {
            args = new object[] {context};
          }
          else
          {
            var dto = ZeroFormatterSerializer.NonGeneric.Deserialize(kv.Value.MessageType, dtoRaw);
            args = new object[] {context, dto};
          }

          _handlersMethodInfo[kv.Key]?.Invoke(instance, args);
        })
      );
    }

    private bool TryProcessMessage(string route, Context context, byte[] dtoRaw)
    {
      if (!_handlers.ContainsKey(route))
      {
        return false;
      }

      _handlers[route](context, dtoRaw);
      return true;
    }
  }

  public interface IProcessingService
  {
    void ProcessMessage(string route, NetIncomingMessage message, bool failWhenNoHandlers = true);
    void ProcessDisconnected(NetConnection connection);
    void ProcessConnected(NetConnection connection);
  }
}