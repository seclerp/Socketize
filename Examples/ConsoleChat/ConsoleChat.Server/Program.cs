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
    class Program
    {
        private static ConcurrentDictionary<IPEndPoint, string> NicknameLookup = new ConcurrentDictionary<IPEndPoint, string>();

        private static ChatState State = new ChatState { Messages = new ConcurrentBag<Message>() };

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

        private static void ProduceChatMessage(ConnectionContext context, string message, string nickname = null, bool sendToCurrentRemotePeer = true)
        {
            nickname ??= "Server";

            var messageModel = new Message { Nickname = nickname, Content = message };
            State.Messages.Add(messageModel);

            var messageDto = new NewMessageDto { Nickname = nickname, Content = message };

            if (sendToCurrentRemotePeer)
            {
                context.SendToAll(MessageNames.NewMessage, messageDto);
            }
            else
            {
                context.SendToOthers(MessageNames.NewMessage, messageDto);
            }
        }

        // When user sends his nickname
        private static void OnNickname(ConnectionContext context, NicknameDto nicknameDto)
        {
            var nickname = nicknameDto.Value;
            NicknameLookup[context.Connection.RemoteEndPoint] = nickname;

            ProduceChatMessage(context, $"'{nickname}' just joined conversation!", null, false);
            context.Send(MessageNames.SyncState, ToChatStateDto(State));
        }

        // When peer was disconnected from the server
        private static void OnDisconnect(ConnectionContext context)
        {
            var userEndpoint = context.Connection.RemoteEndPoint;
            if (NicknameLookup.ContainsKey(userEndpoint))
            {
                var nickname = NicknameLookup[userEndpoint];
                NicknameLookup.Remove(userEndpoint, out _);
                ProduceChatMessage(context, $"'{nickname}' left conversation");
            }

            Console.WriteLine($"'{userEndpoint}' disconnected");
        }

        // When new message is received
        private static void OnSendMessage(ConnectionContext context, SendMessageDto sendMessageDto)
        {
            var userEndpoint = context.Connection.RemoteEndPoint;
            if (!NicknameLookup.ContainsKey(userEndpoint))
            {
                Console.WriteLine($"Ignoring message from '{userEndpoint}' because user don't sent nickname");
                return;
            }

            var nickName = NicknameLookup[userEndpoint];

            // Send message to connected users
            Console.WriteLine($"Received new message from '{nickName}', sending to other users");
            ProduceChatMessage(context, sendMessageDto.Value, nickName);
        }

        static void Main(string[] args)
        {
            // This is needed by Lidgren
            if (SynchronizationContext.Current is null)
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var appId = configuration.GetValue<string>("AppId");
            var port = configuration.GetValue<int>("Port");
            var serverOptions = new ServerOptions(port, appId);

            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddSocketizeServer(
                        schema => schema
                            .Route<NicknameDto>(MessageNames.Nickname, OnNickname)
                            .Route<SendMessageDto>(MessageNames.SendMessage, OnSendMessage)
                            .OnDisconnect(OnDisconnect),
                        serverOptions)
                    .AddSocketizeHosting())
                .Build()
                .Run();
        }
    }
}