using System;
using Microsoft.Extensions.DependencyInjection;
using Socketize.Core.Abstractions;

namespace Socketize.DependencyInjection
{
    /// <summary>
    /// Message handler factory that retrieves message handler objects from dependency injection container.
    /// </summary>
    public class InjectedMessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectedMessageHandlerFactory"/> class.
        /// </summary>
        /// <param name="services">Service locator instance.</param>
        public InjectedMessageHandlerFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public TMessageHandler Get<TMessageHandler>() =>
            _services.GetService<TMessageHandler>();

        /// <inheritdoc />
        public object Get(Type type) =>
            _services.GetService(type);
    }
}