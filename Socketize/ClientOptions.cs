namespace Socketize
{
  public class ClientOptions : Options
  {
    public string ServerHost { get; }
    public int ServerPort { get; }

    public ClientOptions(string serverHost, int serverPort, string appId) : base(appId)
    {
      ServerHost = serverHost;
      ServerPort = serverPort;
    }
  }
}