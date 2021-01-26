using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socketize.Core.Abstractions;
using Socketize.Core.Enums;
using Socketize.Core.Extensions;
using Socketize.Core.Routing.Abstractions;

namespace Socketize.Core.Routing
{
    /// <summary>
    /// Hub schema builder implementation.
    /// </summary>
    public class SchemaHubBuilder : IRoutesBuilder<SchemaHubBuilder>,
        IBuilder<IEnumerable<SchemaItem>>,
        IAsyncRoutesBuilder<SchemaHubBuilder>,
        IHubBuilder<SchemaHubBuilder>
    {
        private readonly string _baseRoute;
        private readonly SchemaHubBuilder _parentHubBuilder;
        private readonly ICollection<SchemaItem> _intermediateItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaHubBuilder"/> class.
        /// </summary>
        /// <param name="baseRoute">Base route for hub.</param>
        /// <param name="parentHubBuilder">Parent hub builder instance.</param>
        public SchemaHubBuilder(string baseRoute, SchemaHubBuilder parentHubBuilder)
        {
            _baseRoute = baseRoute;
            _parentHubBuilder = parentHubBuilder;
            _intermediateItems = new LinkedList<SchemaItem>();
        }

        /// <inheritdoc />
        public SchemaHubBuilder Route<TMessageHandler>(string route)
            where TMessageHandler : IMessageHandler
        {
            AddRoute(route, typeof(TMessageHandler), null, HandlerInstanceKind.Class);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder Route<TMessage, TMessageHandler>(string route)
            where TMessageHandler : IMessageHandler<TMessage>
        {
            AddRoute(route, typeof(TMessageHandler), typeof(TMessage), HandlerInstanceKind.Class);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder Route(string route, Action<ConnectionContext> handlerDelegate)
        {
            AddRoute(route, handlerDelegate.Method, null, HandlerInstanceKind.Delegate);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder Route<TMessage>(string route, Action<ConnectionContext, TMessage> handlerDelegate)
        {
            AddRoute(route, handlerDelegate.Method, typeof(TMessage), HandlerInstanceKind.Delegate);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder AsyncRoute<TMessageHandler>(string route)
            where TMessageHandler : IAsyncMessageHandler
        {
            AddRoute(route, typeof(TMessageHandler), null, HandlerInstanceKind.Class);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder AsyncRoute<TMessage, TMessageHandler>(string route)
            where TMessageHandler : IAsyncMessageHandler<TMessage>
        {
            AddRoute(route, typeof(TMessageHandler), typeof(TMessage), HandlerInstanceKind.Class);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder AsyncRoute(string route, Func<ConnectionContext, Task> handlerDelegate)
        {
            AddRoute(route, handlerDelegate.Method, null, HandlerInstanceKind.Delegate);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder AsyncRoute<TMessage>(string route, Func<ConnectionContext, TMessage, Task> handlerDelegate)
        {
            AddRoute(route, handlerDelegate.Method, typeof(TMessage), HandlerInstanceKind.Delegate);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder Hub(string hubRoute, Func<SchemaHubBuilder, SchemaHubBuilder> hubConfiguration)
        {
            var hubBuilder = new SchemaHubBuilder(_baseRoute.CombineWith(hubRoute), _parentHubBuilder);
            hubConfiguration(hubBuilder);
            var items = hubBuilder.Build();
            foreach (var hubPartItem in items)
            {
                _intermediateItems.Add(hubPartItem);
            }

            return this;
        }

        /// <inheritdoc />
        public IEnumerable<SchemaItem> Build() =>
            _intermediateItems;

        private void AddRoute(string route, object handler, Type messageType, HandlerInstanceKind kind)
        {
            var combinedRoute = _baseRoute.CombineWith(route);
            var newSchemaItem = new SchemaItem(combinedRoute, handler, messageType, kind);
            _intermediateItems.Add(newSchemaItem);
        }
    }
}