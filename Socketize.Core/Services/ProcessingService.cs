using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lidgren.Network;
using Socketize.Core.Abstractions;
using Socketize.Core.Exceptions;
using Socketize.Core.Routing;
using Socketize.Core.Services.Abstractions;
using ZeroFormatter;

namespace Socketize.Core.Services
{
    /// <summary>
    /// Service that process incoming messages.
    /// </summary>
    public class ProcessingService : IProcessingService
    {
        private readonly IDictionary<string, MethodInfo> _handlersMethodInfo;
        private readonly IDictionary<string, Action<ConnectionContext, byte[]>> _handlers;
        private readonly IMessageHandlerFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingService"/> class.
        /// </summary>
        /// <param name="schema">Schema object representing routes and their handlers.</param>
        /// <param name="factory">Factory instance that creates message handlers using type information.</param>
        public ProcessingService(Schema schema, IMessageHandlerFactory factory)
        {
            _factory = factory;
            var itemByRoute = schema.ToDictionary(item => item.Route, item => item);

            _handlersMethodInfo = CreateMessageHandlersMethodInfo(itemByRoute);
            _handlers = CreateMessageHandlers(itemByRoute);
        }

        /// <inheritdoc />
        public void ProcessMessage(string route, NetIncomingMessage message, bool failWhenNoHandlers = true)
        {
            var messageLength = message.ReadInt32();
            var dtoRaw = messageLength is 0 ? null : message.ReadBytes(messageLength);
            var context = new ConnectionContext(message.SenderConnection);

            if (!TryProcessMessage(route, context, dtoRaw) && failWhenNoHandlers)
            {
                throw new SocketizeException($"Handler for route '{route}' not found");
            }
        }

        private IDictionary<string, MethodInfo> CreateMessageHandlersMethodInfo(
            IDictionary<string, SchemaItem> schemaItems) =>
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
                new Action<ConnectionContext, byte[]>((context, dtoRaw) =>
                {
                    var instance = _factory.Get(kv.Value.HandlerType);

                    object[] args;
                    if (kv.Value.MessageType is null)
                    {
                        args = new object[] { context };
                    }
                    else
                    {
                        var dto = ZeroFormatterSerializer.NonGeneric.Deserialize(kv.Value.MessageType, dtoRaw);
                        args = new[] { context, dto };
                    }

                    _handlersMethodInfo[kv.Key]?.Invoke(instance, args);
                }));
        }

        private bool TryProcessMessage(string route, ConnectionContext connectionContext, byte[] dtoRaw)
        {
            if (!_handlers.ContainsKey(route))
            {
                return false;
            }

            _handlers[route](connectionContext, dtoRaw);
            return true;
        }
    }
}