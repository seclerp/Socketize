using System;
using System.Linq;
using Socketize.Core.Abstractions;
using Socketize.Core.Routing.Abstractions;

namespace Socketize.Core.Routing
{
    /// <summary>
    /// Root schema builder implementation.
    /// </summary>
    public class SchemaBuilder : IBuilder<Schema>, ISchemaBuilder<SchemaBuilder>
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