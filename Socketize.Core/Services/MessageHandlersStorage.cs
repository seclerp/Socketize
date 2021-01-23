using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Socketize.Core.Abstractions;
using Socketize.Core.Enums;
using Socketize.Core.Routing;
using ZeroFormatter;

namespace Socketize.Core.Services
{
    public class MessageHandlersStorage : IMessageHandlersStorage
    {
        private readonly IMessageHandlerFactory _factory;
        private IDictionary<string, MethodInfo> _classHandlersMethodInfo;
        private IDictionary<string, Action<ConnectionContext, byte[]>> _classHandlers;

        public MessageHandlersStorage(Schema schema, IMessageHandlerFactory factory)
        {
            _factory = factory;

            PopulateMethodsInfo(schema);
        }

        public void Invoke(string route, ConnectionContext context, byte[] dtoRaw)
        {
            _classHandlers[route].Invoke(context, dtoRaw);
        }

        public bool HasRoute(string route)
        {
            return _classHandlers.ContainsKey(route);
        }

        private static bool IsValidHandler(MethodInfo method, Type messageType)
        {
            var parameters = method.GetParameters();
            if (messageType is null
                && parameters.Length == 1
                && parameters[0].ParameterType.IsAssignableFrom(typeof(ConnectionContext)))
            {
                return true;
            }

            if (parameters.Length == 2
                && parameters[0].ParameterType.IsAssignableFrom(typeof(ConnectionContext))
                && parameters[1].ParameterType.IsAssignableFrom(messageType))
            {
                return true;
            }

            return false;
        }

        private IDictionary<string, Action<ConnectionContext, byte[]>> CreateMessageHandlers(
            IDictionary<string, SchemaItem> schemaItems)
        {
            return schemaItems.ToDictionary(kv => kv.Key, kv =>
                kv.Value.Kind switch
                {
                    HandlerInstanceKind.Class =>
                        new Action<ConnectionContext, byte[]>((context, dtoRaw) =>
                        {
                            var instance = _factory.Get(kv.Value.Handler as Type);
                            InvokeMessageHandler(kv.Value, instance, context, dtoRaw);
                        }),
                    HandlerInstanceKind.Delegate =>
                        new Action<ConnectionContext, byte[]>((context, dtoRaw) =>
                            InvokeMessageHandler(kv.Value, default, context, dtoRaw)),
                    _ => default,
                });
        }

        private void InvokeMessageHandler(SchemaItem item, object instance, ConnectionContext context, byte[] dtoRaw)
        {
            object[] args;
            if (item.MessageType is null)
            {
                args = new object[] { context };
            }
            else
            {
                var dto = ZeroFormatterSerializer.NonGeneric.Deserialize(item.MessageType, dtoRaw);
                args = new[] { context, dto };
            }

            _classHandlersMethodInfo[item.Route]?.Invoke(instance, args);
        }

        private IDictionary<string, MethodInfo> CreateMessageHandlersMethodInfo(
            IDictionary<string, SchemaItem> schemaItems) =>
            schemaItems.ToDictionary(kv => kv.Key, kv =>
                ((Type)kv.Value.Handler)
                    .GetMethods()
                    .Where(info => info.Name is "Handle")
                    .FirstOrDefault(methodInfo => IsValidHandler(methodInfo, kv.Value.MessageType)));

        private void PopulateMethodsInfo(Schema schema)
        {
            var classItemsByRoute = schema
                .Where(item => item.Kind is HandlerInstanceKind.Class)
                .ToDictionary(item => item.Route, item => item);

            var classMessageHandlersMethodInfo = CreateMessageHandlersMethodInfo(classItemsByRoute);

            var delegateMessageHandlesMethodInfo = schema
                .Where(item => item.Kind is HandlerInstanceKind.Delegate)
                .ToDictionary(item => item.Route, item => item.Handler as MethodInfo)
                    as IDictionary<string, MethodInfo>;

            _classHandlersMethodInfo =
                classMessageHandlersMethodInfo
                    .Concat(delegateMessageHandlesMethodInfo)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

            _classHandlers = CreateMessageHandlers(classItemsByRoute);
        }
    }
}