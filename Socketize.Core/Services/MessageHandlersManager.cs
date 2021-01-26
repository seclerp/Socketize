using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Socketize.Core.Abstractions;
using Socketize.Core.Enums;
using Socketize.Core.Routing;
using Socketize.Core.Services.Abstractions;
using ZeroFormatter;

namespace Socketize.Core.Services
{
    /// <summary>
    /// Service that that represents manager for message handlers.
    /// </summary>
    public class MessageHandlersManager : IMessageHandlersManager
    {
        private readonly IMessageHandlerFactory _factory;
        private IDictionary<string, MethodInfo> _classHandlersMethodInfo;
        private IDictionary<string, Func<ConnectionContext, byte[], Task>> _classHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlersManager"/> class.
        /// </summary>
        /// <param name="schema">Schema instance.</param>
        /// <param name="factory">Factory for message handlers.</param>
        public MessageHandlersManager(Schema schema, IMessageHandlerFactory factory)
        {
            _factory = factory;

            PopulateMethodsInfo(schema);
        }

        /// <inheritdoc />
        public Task Invoke(string route, ConnectionContext context, byte[] dtoRaw)
        {
            return _classHandlers[route].Invoke(context, dtoRaw);
        }

        /// <inheritdoc />
        public bool RouteExists(string route)
        {
            return _classHandlers.ContainsKey(route);
        }

        private static bool IsHandlerValid(MethodInfo method, Type messageType)
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

        private static object[] PrepareArguments(SchemaItem item, ConnectionContext context, byte[] rawDto)
        {
            object[] args;
            if (item.MessageType is null)
            {
                args = new object[] { context };
            }
            else
            {
                var dto = ZeroFormatterSerializer.NonGeneric.Deserialize(item.MessageType, rawDto);
                args = new[] { context, dto };
            }

            return args;
        }

        private IDictionary<string, Func<ConnectionContext, byte[], Task>> CreateMessageHandlers(
            IDictionary<string, SchemaItem> schemaItems)
        {
            return schemaItems.ToDictionary(kv => kv.Key, kv =>
            {
                var item = kv.Value;
                switch (kv.Value.Kind)
                {
                    case HandlerInstanceKind.Class:
                        var classReturnType = _classHandlersMethodInfo[item.Route].ReturnType;
                        return CreateClassMessageHandler(item, classReturnType);
                    case HandlerInstanceKind.Delegate:
                        var delegateReturnType = _classHandlersMethodInfo[kv.Value.Route].ReturnType;
                        return CreateDelegateMessageHandler(item, delegateReturnType);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private Func<ConnectionContext, byte[], Task> CreateClassMessageHandler(SchemaItem item, Type handlerReturnType)
        {
            return handlerReturnType switch
            {
                var type when type == typeof(Task) =>
                    (context, dtoRaw) =>
                    {
                        var instance = _factory.Get(item.Handler as Type);
                        return InvokeAsyncMessageHandler(item, instance, context, dtoRaw);
                    },
                var type when type == typeof(void) =>
                    (context, dtoRaw) =>
                    {
                        var instance = _factory.Get(item.Handler as Type);
                        InvokeMessageHandler(item, instance, context, dtoRaw);
                        return Task.CompletedTask;
                    },
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private Func<ConnectionContext, byte[], Task> CreateDelegateMessageHandler(SchemaItem item, Type handlerReturnType)
        {
            return handlerReturnType switch
            {
                var type when type == typeof(Task) =>
                    (context, dtoRaw) =>
                        InvokeAsyncMessageHandler(item, default, context, dtoRaw),
                var type when type == typeof(void) =>
                    (context, dtoRaw) =>
                    {
                        InvokeAsyncMessageHandler(item, default, context, dtoRaw);
                        return Task.CompletedTask;
                    },
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private void InvokeMessageHandler(SchemaItem item, object instance, ConnectionContext context, byte[] dtoRaw)
        {
            var args = PrepareArguments(item, context, dtoRaw);

            _classHandlersMethodInfo[item.Route]?.Invoke(instance, args);
        }

        private Task InvokeAsyncMessageHandler(SchemaItem item, object instance, ConnectionContext context, byte[] dtoRaw)
        {
            var args = PrepareArguments(item, context, dtoRaw);

            return (Task)_classHandlersMethodInfo[item.Route]?.Invoke(instance, args);
        }

        private IDictionary<string, MethodInfo> CreateMessageHandlersMethodInfo(
            IDictionary<string, SchemaItem> schemaItems) =>
            schemaItems.ToDictionary(kv => kv.Key, kv =>
                ((Type)kv.Value.Handler)
                    .GetMethods()
                    .Where(info => info.Name is "Handle")
                    .FirstOrDefault(methodInfo => IsHandlerValid(methodInfo, kv.Value.MessageType)));

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