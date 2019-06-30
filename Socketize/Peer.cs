using System;
using System.Net;
using System.Threading;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Socketize.Abstractions;

namespace Socketize
{
  public abstract class Peer : IPeer
  {
    private readonly IProcessingService _processingService;
    internal readonly ILogger<Peer> Logger;

    public Context CreateRemoteContext(IPEndPoint target) =>
      new Context(NetPeer.GetConnection(target));

    public NetPeer NetPeer { get; private set; }

    public Peer(IProcessingService processingService, ILogger<Peer> logger)
    {
      _processingService = processingService;
      Logger = logger;
    }

    public abstract NetPeer GetPeer();

    public virtual void Start()
    {
      Logger.LogInformation("Peer starting");
      NetPeer = GetPeer();
      NetPeer.RegisterReceivedCallback(ProcessMessage);
      NetPeer.Start();
      Logger.LogInformation("Peer started");
    }

    public virtual void Stop()
    {
      Logger.LogInformation("Peer stopping");
      NetPeer.Shutdown("Peer stopping");
      Logger.LogInformation("Peer stopped");
    }

    private void ProcessMessage(object peer)
    {
      ThreadPool.QueueUserWorkItem(_ =>
      {
        try
        {
          var message = ((NetPeer) peer).ReadMessage();
          ProcessIncomingMessageType(message);
        }
        catch (Exception ex)
        {
          Logger.LogError(ex, "Error while processing message");
        }
      });
    }

    private void ProcessIncomingMessageType(NetIncomingMessage message)
    {
      Logger.LogInformation($"Received message with type '{message.MessageType.ToString()}', connection status: {message.SenderConnection.Status}");

      switch (message.MessageType)
      {
        case NetIncomingMessageType.ConnectionApproval:
          ProcessConnectionApproval(message);
          break;
        case NetIncomingMessageType.StatusChanged:
          ProcessStatusChanged(message);
          break;
        case NetIncomingMessageType.Data:
          ProcessData(message);
          break;
      }
    }

    private void ProcessConnectionApproval(NetIncomingMessage netIncomingMessage)
    {
      // TODO: Validate
      netIncomingMessage.SenderConnection.Approve();
      Logger.LogInformation($"Approved connection for endpoint {netIncomingMessage.SenderEndPoint}");
    }

    private void ProcessStatusChanged(NetIncomingMessage message)
    {
      switch (message.SenderConnection.Status)
      {
        case NetConnectionStatus.Connected:
          ProcessConnected(message);
          return;
        case NetConnectionStatus.Disconnected:
          ProcessDisconnected(message);
          return;
      }
    }

    private void ProcessConnected(NetIncomingMessage message)
    {
      _processingService.ProcessConnected(message.SenderConnection);
      Logger.LogInformation($"'{message.SenderConnection.RemoteEndPoint}' connected");
    }

    private void ProcessDisconnected(NetIncomingMessage message)
    {
      _processingService.ProcessDisconnected(message.SenderConnection);
      Logger.LogInformation($"'{message.SenderConnection.RemoteEndPoint}' disconnected");
    }

    private void ProcessData(NetIncomingMessage message)
    {
      var route = message.ReadString();
      _processingService.ProcessMessage(route, message);
      Logger.LogInformation($"Processed message for route '{route}'");
    }
  }
}