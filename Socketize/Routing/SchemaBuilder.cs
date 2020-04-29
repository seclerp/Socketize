using System.Collections.Generic;
using System.Linq;
using Socketize.Abstractions;

namespace Socketize.Routing
{
  public class SchemaBuilder
  {
    private readonly IList<SchemaPartBuilder> _hubBuilders;

    private readonly IList<SchemaItem> _rootItems;

    public SchemaBuilder()
    {
      _hubBuilders = new List<SchemaPartBuilder>();
      _rootItems = new List<SchemaItem>();
    }

    public SchemaPartBuilder Hub(string baseRoute)
    {
      var hubBuilder = new SchemaPartBuilder(baseRoute, this);
      _hubBuilders.Add(hubBuilder);
      return hubBuilder;
    }

    public SchemaBuilder OnConnect<TMessageHandler>() where TMessageHandler : IMessageHandler
    {
      _rootItems.Add(new SchemaItem
      {
        Route = SpecialRouteNames.ConnectRoute,
        HandlerType = typeof(TMessageHandler),
        MessageType = null
      });

      return this;
    }

    public SchemaBuilder OnDisconnect<TMessageHandler>() where TMessageHandler : IMessageHandler
    {
      _rootItems.Add(new SchemaItem
      {
        Route = SpecialRouteNames.DisconnectRoute,
        HandlerType = typeof(TMessageHandler),
        MessageType = null
      });

      return this;
    }

    public SchemaBuilder Route<TMessageHandler>(string route) where TMessageHandler : IMessageHandler
    {
      _rootItems.Add(new SchemaItem
      {
        Route = route,
        HandlerType = typeof(TMessageHandler),
        MessageType = null
      });

      return this;
    }

    public SchemaBuilder Route<TMessage, TMessageHandler>(string route) where TMessageHandler : IMessageHandler<TMessage>
    {
      _rootItems.Add(new SchemaItem
      {
        Route = route,
        HandlerType = typeof(TMessageHandler),
        MessageType = typeof(TMessage)
      });

      return this;
    }

    public Schema BuildSchema() => new Schema
    {
      RootPart = new SchemaPart { Items = _rootItems.ToArray() },
      Parts = _hubBuilders.Select(builder => builder.Build()).ToArray()
    };
  }
}