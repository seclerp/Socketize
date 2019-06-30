using System.Collections.Generic;

namespace Socketize.Routing
{
  public class SchemaPart
  {
    public string Route { get; set; }
    public IReadOnlyCollection<SchemaItem> Items { get; set; }
  }
}