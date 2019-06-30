using Socketize.Routing;

namespace Socketize.Abstractions
{
  public interface IRouteMap
  {
    SchemaBuilder Declare(SchemaBuilder builder);
  }
}