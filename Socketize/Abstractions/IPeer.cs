using System.Net;
using Lidgren.Network;

namespace Socketize.Abstractions
{
  public interface IPeer
  {
    void Start();
    void Stop();
    Context CreateRemoteContext(IPEndPoint target);
    NetPeer NetPeer { get; }
  }
}