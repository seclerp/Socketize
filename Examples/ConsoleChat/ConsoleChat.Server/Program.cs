using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        // When user sends his nickname
        private static void OnNickname(ConnectionContext context, NicknameDto nicknameDto)
        {
            var nickname = nicknameDto.Value;
            NicknameLookup[context.Connection.RemoteEndPoint] = nickname;

            context.SendToAll(MessageNames.UserJoined, new UserJoinedDto { Value = nickname });
        }

        // When peer was disconnected from the server
        private static void OnDisconnect(ConnectionContext context)
        {
            var userEndpoint = context.Connection.RemoteEndPoint;
            if (NicknameLookup.ContainsKey(userEndpoint))
            {
                var nickname = NicknameLookup[userEndpoint];
                NicknameLookup.Remove(userEndpoint, out _);
                context.SendToAll(MessageNames.UserLeft, new UserLeftDto { Value = nickname });
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
            context.SendToOthers(MessageNames.NewMessage, new NewMessageDto { Nickname = nickName, Content = sendMessageDto.Value });
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