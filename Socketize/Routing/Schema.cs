using System.Collections.Generic;

namespace Socketize.Routing
{
  public class Schema
  {
    public IReadOnlyCollection<SchemaPart> Parts { get; set; }
    public SchemaPart Special { get; set; }
  }
}