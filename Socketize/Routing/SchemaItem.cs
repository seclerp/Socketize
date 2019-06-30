using System;

namespace Socketize.Routing
{
  public class SchemaItem
  {
    public string Route { get; set; }
    public Type HandlerType { get; set; }
    public Type MessageType { get; set; }
  }
}