using System.Collections.Generic;
using System.Linq;
using Socketize.Abstractions;

namespace Socketize.Routing
{
  public class SchemaBuilder
  {
    private readonly IList<SchemaPartBuilder> _hubBuilders;

    private readonly IList<SchemaItem> _specialItems;

    public SchemaBuilder()
    {
      _hubBuilders = new List<SchemaPartBuilder>();
      _specialItems = new List<SchemaItem>();
    }

    public SchemaPartBuilder Hub(string baseRoute)
    {
      var hubBuilder = new SchemaPartBuilder(baseRoute, this);
      _hubBuilders.Add(hubBuilder);
      return hubBuilder;
    }

    public SchemaBuilder OnConnect<TMessageHandler>() where TMessageHandler : IMessageHandler
    {
      _specialItems.Add(new SchemaItem
      {
        Route = SpecialRouteNames.ConnectRoute,
        HandlerType = typeof(TMessageHandler),
        MessageType = null
      });

      return this;
    }

    public SchemaBuilder OnDisconnect<TMessageHandler>() where TMessageHandler : IMessageHandler
    {
      _specialItems.Add(new SchemaItem
      {
        Route = SpecialRouteNames.DisconnectRoute,
        HandlerType = typeof(TMessageHandler),
        MessageType = null
      });

      return this;
    }

    public Schema BuildSchema() => new Schema
    {
      Special = new SchemaPart { Items = _specialItems.ToArray() },
      Parts = _hubBuilders.Select(builder => builder.Build()).ToArray()
    };
  }
}