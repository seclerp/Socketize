using System.Collections.Generic;
using System.Linq;
using Socketize.Abstractions;

namespace Socketize.Routing
{
  public class SchemaPartBuilder
  {
    private readonly string _baseRoute;
    private readonly SchemaBuilder _parentBuilder;
    private IList<SchemaItem> _intermediateItems;

    public SchemaPartBuilder(string baseRoute, SchemaBuilder parentBuilder)
    {
      _baseRoute = baseRoute;
      _parentBuilder = parentBuilder;
      _intermediateItems = new List<SchemaItem>();
    }

    public SchemaPartBuilder Route<TMessageHandler>(string route) where TMessageHandler : IMessageHandler
    {
      _intermediateItems.Add(new SchemaItem { Route = CombineRoutes(_baseRoute, route), MessageType = null, HandlerType = typeof(TMessageHandler) });

      return this;
    }

    public SchemaPartBuilder Route<TMessage, TMessageHandler>(string route) where TMessageHandler : IMessageHandler<TMessage>
    {
      _intermediateItems.Add(new SchemaItem { Route = CombineRoutes(_baseRoute, route), MessageType = typeof(TMessage), HandlerType = typeof(TMessageHandler) });

      return this;
    }

    public SchemaBuilder Complete() => _parentBuilder;

    public SchemaPart Build() => new SchemaPart { Route = _baseRoute, Items = _intermediateItems.ToArray() };

    private static string CombineRoutes(string routeA, string routeB) => $"{routeA}/{routeB}";
  }
}