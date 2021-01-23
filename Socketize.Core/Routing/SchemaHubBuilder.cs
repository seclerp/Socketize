using System;
using System.Collections.Generic;
using Socketize.Core.Abstractions;
using Socketize.Core.Enums;
using Socketize.Core.Extensions;
using Socketize.Core.Routing.Abstractions;

namespace Socketize.Core.Routing
{
    /// <summary>
    /// Hub schema builder implementation.
    /// </summary>
    public class SchemaHubBuilder : ISchemaBuilder<SchemaHubBuilder>, IBuilder<IEnumerable<SchemaItem>>
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
            var combinedRoute = _baseRoute.CombineWith(route);
            var newSchemaItem = new SchemaItem(combinedRoute, typeof(TMessageHandler), null, HandlerInstanceKind.Class);
            _intermediateItems.Add(newSchemaItem);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder Route<TMessage, TMessageHandler>(string route)
            where TMessageHandler : IMessageHandler<TMessage>
        {
            var combinedRoute = _baseRoute.CombineWith(route);
            var newSchemaItem = new SchemaItem(combinedRoute, typeof(TMessageHandler), typeof(TMessage), HandlerInstanceKind.Class);
            _intermediateItems.Add(newSchemaItem);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder Route(string route, Action<ConnectionContext> handlerDelegate)
        {
            var combinedRoute = _baseRoute.CombineWith(route);
            var newSchemaItem = new SchemaItem(combinedRoute, handlerDelegate.Method, null, HandlerInstanceKind.Delegate);
            _intermediateItems.Add(newSchemaItem);

            return this;
        }

        /// <inheritdoc />
        public SchemaHubBuilder Route<TMessage>(string route, Action<ConnectionContext, TMessage> handlerDelegate)
        {
            var combinedRoute = _baseRoute.CombineWith(route);
            var newSchemaItem = new SchemaItem(combinedRoute, handlerDelegate.Method, typeof(TMessage), HandlerInstanceKind.Delegate);
            _intermediateItems.Add(newSchemaItem);

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
    }
}