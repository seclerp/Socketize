using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using ConsoleChat.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Socketize.Core;
using Socketize.Core.Extensions;
using Socketize.Server.AspNetCore;
using Socketize.Server.Configuration;
using Socketize.Server.DependencyInjection;

namespace ConsoleChat.Server
{
    /// <summary>
    /// Main entry class.
    /// </summary>
    public class Program
    {
        // Better idea is to move _state and _nicknameLookup to some cache or persistence storage, but for our example lets use just a

        // Lookup that mao user IP endpoint to a nickname, used to determine message owner's nickname
        private static ConcurrentDictionary<IPEndPoint, string> _nicknameLookup =
            new ConcurrentDictionary<IPEndPoint, string>();

        // Current server state, in our example it is just
        private static ChatState _state = new ChatState { Messages = new ConcurrentQueue<Message>() };

        /// <summary>
        /// Maps chat state into DTO for sending to a user.
        /// </summary>
        /// <param name="state">Server chat state object.</param>
        /// <returns>Server chat state DTO for sending to a user.</returns>
        private static SyncStateDto ToChatStateDto(ChatState state)
        {
            return new SyncStateDto
            {
                Messages = state.Messages.Select(message => new MessageStateDto
                {
                    Nickname = message.Nickname,
                    Content = message.Content,
                }),
            };
        }

        /// <summary>
        /// Adds message to server messages and sends it to interested users.
        /// </summary>
        /// <param name="context">Connected context for a currently processing message.</param>
        /// <param name="message">Message to be produced.</param>
        /// <param name="nickname">Nickname of a user. If null, Server name will be used.</param>
        /// <param name="sendToCurrentRemotePeer">If true, peer that sent message will also receive produced message.</param>
        private static void ProduceChatMessage(ConnectionContext context, string message, string nickname = null, bool sendToCurrentRemotePeer = true)
        {
            nickname ??= "Server";

            var messageModel = new Message { Nickname = nickname, Content = message };
            _state.Messages.Enqueue(messageModel);

            var messageDto = new NewMessageDto { Nickname = nickname, Content = message };

            if (sendToCurrentRemotePeer)
            {
                context.SendToAll(RouteNames.NewMessage, messageDto);
            }
            else
            {
                context.SendToOthers(RouteNames.NewMessage, messageDto);
            }
        }

        /// <summary>
        /// Called when new client sent nickname.
        /// </summary>
        /// <param name="context">Connection context to the server.</param>
        /// <param name="nicknameDto">DTO that contains payload.</param>
        private static void OnNickname(ConnectionContext context, NicknameDto nicknameDto)
        {
            var nickname = nicknameDto.Value;
            _nicknameLookup[context.Connection.RemoteEndPoint] = nickname;

            ProduceChatMessage(context, $"'{nickname}' just joined conversation!", null, false);
            context.Send(RouteNames.SyncState, ToChatStateDto(_state));
        }

        /// <summary>
        /// Called when client disconnected from a server.
        /// </summary>
        /// <param name="context">Connection context to the server.</param>
        private static void OnDisconnect(ConnectionContext context)
        {
            var userEndpoint = context.Connection.RemoteEndPoint;
            if (_nicknameLookup.ContainsKey(userEndpoint))
            {
                var nickname = _nicknameLookup[userEndpoint];
                _nicknameLookup.Remove(userEndpoint, out _);
                ProduceChatMessage(context, $"'{nickname}' left conversation");
            }
        }

        /// <summary>
        /// Called when client sent chat message.
        /// </summary>
        /// <param name="context">Connection context to the server.</param>
        /// <param name="sendMessageDto">DTO that contains payload.</param>
        private static void OnSendMessage(ConnectionContext context, SendMessageDto sendMessageDto)
        {
            var userEndpoint = context.Connection.RemoteEndPoint;
            if (!_nicknameLookup.ContainsKey(userEndpoint))
            {
                Console.WriteLine($"Ignoring message from '{userEndpoint}' because user don't sent nickname");
                return;
            }

            var nickName = _nicknameLookup[userEndpoint];

            // Send message to connected users
            Console.WriteLine($"Received new message from '{nickName}', sending to other users");
            ProduceChatMessage(context, sendMessageDto.Value, nickName);
        }

        private static void Main(string[] args)
        {
            // This is needed by Lidgren
            // TODO: Move it inside framework
            if (SynchronizationContext.Current is null)
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }

            // Use configuration builder to simplify application configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            // Get parameters needed for connection from configuration
            var appId = configuration.GetValue<string>("AppId");
            var port = configuration.GetValue<int>("Port");

            // Create server options object that will be used for creating server
            var serverOptions = new ServerOptions(port, appId);

            // We using generic host builder to simplify Socketize server hosting, but it is not mandatory
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddSocketizeServer(
                        schema => schema
                            .Route<NicknameDto>(RouteNames.Nickname, OnNickname)
                            .Route<SendMessageDto>(RouteNames.SendMessage, OnSendMessage)
                            .OnDisconnect(OnDisconnect),
                        serverOptions)
                    .AddSocketizeHosting())
                .Build()
                .Run();
        }
    }
}