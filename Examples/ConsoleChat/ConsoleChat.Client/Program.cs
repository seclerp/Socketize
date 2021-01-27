using System;
using System.Net;
using System.Threading;
using ConsoleChat.Contract;
using Lidgren.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Socketize.Client;
using Socketize.Client.Configuration;
using Socketize.Client.DependencyInjection;
using Socketize.Core;
using Socketize.Core.Abstractions;
using Socketize.Core.Extensions;

namespace ConsoleChat.Client
{
    class Program
    {
        static void OnUserJoined(ConnectionContext context, UserJoinedDto userJoinedDto)
        {
            Console.WriteLine($"User '{userJoinedDto.Value}' joined conversation!");
        }

        static void OnUserLeft(ConnectionContext context, UserLeftDto userLeftDto)
        {
            Console.WriteLine($"User '{userLeftDto.Value}' left conversation.");
        }

        static void OnNewMessage(ConnectionContext context, NewMessageDto newMessageDto)
        {
            Console.WriteLine($"[{newMessageDto.Nickname}]: {newMessageDto.Content}");
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
            var host = configuration.GetValue<string>("Host");
            var port = configuration.GetValue<int>("Port");
            var clientOptions = new ClientOptions(host, port, appId);

            var services = new ServiceCollection();
            services.AddSocketizeClient(
                schema => schema
                    .Route<UserJoinedDto>(MessageNames.UserJoined, OnUserJoined)
                    .Route<UserLeftDto>(MessageNames.UserLeft, OnUserLeft)
                    .Route<NewMessageDto>(MessageNames.NewMessage, OnNewMessage),
                clientOptions);
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var serviceProvider = services.BuildServiceProvider();

            Console.Write("Please enter your nickname: ");
            var nickName = Console.ReadLine();

            var client = serviceProvider.GetService<ClientPeer>();
            client.Start();

            client.ServerContext.Send(MessageNames.Nickname, new NicknameDto { Value = nickName });

            while (true)
            {
                var newMessage = Console.ReadLine();
                client.ServerContext.Send(MessageNames.SendMessage, new SendMessageDto { Value = newMessage });
            }
        }
    }
}