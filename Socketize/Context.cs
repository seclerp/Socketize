using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lidgren.Network;
using ZeroFormatter;

namespace Socketize
{
  public class Context
  {
    public NetConnection Other { get; }

    public IEnumerable<NetConnection> All => Other.Peer.Connections;
    public IEnumerable<NetConnection> Others => Other.Peer.Connections.Where(conn => conn != Other);

    public Context(NetConnection other)
    {
      Other = other;
    }

    public void Send<T>(string route, T messageDto) =>
      SendInternal(route, messageDto, Other);

    public void SendTo<T>(IPEndPoint endpoint, string route, T messageDto) =>
      SendInternal(route, messageDto, Other.Peer.GetConnection(endpoint));

    public void SendToAll<T>(string route, T messageDto) =>
      SendInternal(route, messageDto, All);

    public void SendToOthers<T>(string route, T messageDto) =>
      SendInternal(route, messageDto, Others);

    private void SendInternal<T>(string route, T messageDto, IEnumerable<NetConnection> connections) =>
      Parallel.ForEach(connections, connection => SendInternal(route, messageDto, connection));

    private void SendInternal<T>(string route, T messageDto, NetConnection connection)
    {
      var message = PrepareRawMessage(route, messageDto);
      connection.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 0);
    }

    private NetOutgoingMessage PrepareRawMessage<T>(string route, T messageDto)
    {
      var dtoRaw = ZeroFormatterSerializer.Serialize(messageDto);
      var message = Other.Peer.CreateMessage();
      message.Write(route);
      message.Write(dtoRaw.Length);
      if (dtoRaw.Length != 0)
      {
        message.Write(dtoRaw);
      }

      return message;
    }
  }
}