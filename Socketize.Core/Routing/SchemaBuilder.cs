using System;
using System.Linq;
using System.Threading.Tasks;
using Socketize.Core.Abstractions;
using Socketize.Core.Routing.Abstractions;

namespace Socketize.Core.Routing
{
    /// <summary>
    /// Root schema builder implementation.
    /// </summary>
    public class SchemaBuilder : IBuilder<Schema>,
        IRoutesBuilder<SchemaBuilder>,
        IAsyncRoutesBuilder<SchemaBuilder>,
        IHubBuilder<SchemaBuilder>
    {
        private readonly SchemaHubBuilder _rootHubBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaBuilder"/> class.
        /// </summary>
        private SchemaBuilder()
        {
            // Inner builder is just a builder for a hub without base address and a parent builder.
            _rootHubBuilder = new SchemaHubBuilder(default, default);
        }

        /// <summary>
        /// Creates new instance of <see cref="SchemaBuilder"/>.
        /// </summary>
        /// <returns>New instance of <see cref="SchemaBuilder"/>.</returns>
        public static SchemaBuilder Create() =>
            new SchemaBuilder();

        /// <inheritdoc />
        public SchemaBuilder Route<TMessageHandler>(string route)
            where TMessageHandler : IMessageHandler
        {
            _rootHubBuilder.Route<TMessageHandler>(route);

            return this;
        }

        /// <inheritdoc />
        public SchemaBuilder Route<TMessage, TMessageHandler>(string route)
            where TMessageHandler : IMessageHandler<TMessage>
        {
            _rootHubBuilder.Route<TMessage, TMessageHandler>(route);

            return this;
        }

        /// <inheritdoc />
        public SchemaBuilder Route(string route, Action<ConnectionContext> handlerDelegate)
        {
            _rootHubBuilder.Route(route, handlerDelegate);

            return this;
        }

        /// <inheritdoc />
        public SchemaBuilder Route<TMessage>(string route, Action<ConnectionContext, TMessage> handlerDelegate)
        {
            _rootHubBuilder.Route(route, handlerDelegate);

            return this;
        }

        /// <inheritdoc />
        public SchemaBuilder AsyncRoute<TMessageHandler>(string route)
            where TMessageHandler : IAsyncMessageHandler
        {
            _rootHubBuilder.AsyncRoute<TMessageHandler>(route);

            return this;
        }

        /// <inheritdoc />
        public SchemaBuilder AsyncRoute<TMessage, TMessageHandler>(string route)
            where TMessageHandler : IAsyncMessageHandler<TMessage>
        {
            _rootHubBuilder.AsyncRoute<TMessage, TMessageHandler>(route);

            return this;
        }

        /// <inheritdoc />
        public SchemaBuilder AsyncRoute(string route, Func<ConnectionContext, Task> handlerDelegate)
        {
            _rootHubBuilder.AsyncRoute(route, handlerDelegate);

            return this;
        }

        /// <inheritdoc />
        public SchemaBuilder AsyncRoute<TMessage>(string route, Func<ConnectionContext, TMessage, Task> handlerDelegate)
        {
            _rootHubBuilder.AsyncRoute(route, handlerDelegate);

            return this;
        }

        /// <inheritdoc />
        public SchemaBuilder Hub(string hubRoute, Func<SchemaHubBuilder, SchemaHubBuilder> hubConfiguration)
        {
            _rootHubBuilder.Hub(hubRoute, hubConfiguration);

            return this;
        }

        /// <inheritdoc />
        public Schema Build() =>
            new Schema(_rootHubBuilder.Build().ToArray());
    }
}