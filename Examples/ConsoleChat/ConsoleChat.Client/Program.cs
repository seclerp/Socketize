using System;
using System.Threading;
using ConsoleChat.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Socketize.Client;
using Socketize.Client.Configuration;
using Socketize.Client.DependencyInjection;
using Socketize.Core;
using Socketize.Core.Extensions;

namespace ConsoleChat.Client
{
    class Program
    {
        private static void PrintNewMessage(string nickname, string content)
        {
            Console.WriteLine($"[{nickname}]: {content}");
        }

        static void OnNewMessage(ConnectionContext context, NewMessageDto newMessageDto)
        {
            PrintNewMessage(newMessageDto.Nickname, newMessageDto.Content);
        }

        static void OnSyncState(ConnectionContext context, SyncStateDto syncStateDto)
        {
            foreach (var message in syncStateDto.Messages)
            {
                PrintNewMessage(message.Nickname, message.Content);
            }
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
                    .Route<NewMessageDto>(MessageNames.NewMessage, OnNewMessage)
                    .Route<SyncStateDto>(MessageNames.SyncState, OnSyncState),
                clientOptions);
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Warning);
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
                ClearPreviousConsoleLine();
                client.ServerContext.Send(MessageNames.SendMessage, new SendMessageDto { Value = newMessage });
            }
        }

        private static void ClearPreviousConsoleLine()
        {
            var currentLineCursor = Console.CursorTop - 1;
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}