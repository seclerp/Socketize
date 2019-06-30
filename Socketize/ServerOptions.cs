namespace Socketize
{
  public class ServerOptions : Options
  {
    public int Port { get; }

    public ServerOptions(int port, string appId) : base(appId)
    {
      Port = port;
    }
  }
}