using Lidgren.Network;
using Microsoft.Extensions.Logging;

namespace Socketize
{
  public class Server : Peer
  {
    private readonly ServerOptions _options;

    public Server(
      IProcessingService processingService,
      ILogger<Server> logger,
      ServerOptions options) : base(processingService, logger)
    {
      _options = options;
    }

    public override NetPeer GetPeer()
    {
      var config = new NetPeerConfiguration(_options.AppId) { Port = _options.Port };
      config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
      config.AcceptIncomingConnections = true;
      return new NetPeer(config);
    }
  }
}