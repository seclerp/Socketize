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
    /// <summary>
    /// Main entry class.
    /// </summary>
    public class Program
    {
        private static void PrintNewMessage(string nickname, string content)
        {
            Console.WriteLine($"[{nickname}]: {content}");
        }

        /// <summary>
        /// Called when new chat message was received from server.
        /// </summary>
        /// <param name="context">Connection context to the server.</param>
        /// <param name="newMessageDto">DTO that contains payload.</param>
        private static void OnNewMessage(ConnectionContext context, NewMessageDto newMessageDto)
        {
            PrintNewMessage(newMessageDto.Nickname, newMessageDto.Content);
        }

        /// <summary>
        /// Called when server sends current chat history and state after connection was estabilished.
        /// </summary>
        /// <param name="context">Connection context to the server.</param>
        /// <param name="syncStateDto">DTO that contains payload.</param>
        private static void OnSyncState(ConnectionContext context, SyncStateDto syncStateDto)
        {
            foreach (var message in syncStateDto.Messages)
            {
                PrintNewMessage(message.Nickname, message.Content);
            }
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
            var host = configuration.GetValue<string>("Host");
            var port = configuration.GetValue<int>("Port");

            // Create client options object that will be used for establishing server connection
            var clientOptions = new ClientOptions(host, port, appId);

            // Dependency injection is used for simplicity, but Socketize could be used without DI if needed, but it more complicated
            var services = new ServiceCollection();
            services.AddSocketizeClient(
                schema => schema
                    .Route<NewMessageDto>(RouteNames.NewMessage, OnNewMessage)
                    .Route<SyncStateDto>(RouteNames.SyncState, OnSyncState),
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

            // Retrieve client peer object to start working with server
            var client = serviceProvider.GetService<ClientPeer>();

            // ... and start it
            client.Start();

            // Send message to nickname route to get server to know about connected user identity
            client.ServerContext.Send(RouteNames.Nickname, new NicknameDto { Value = nickName });

            // Infinite loop for continuous receiving input from user
            while (true)
            {
                var newMessage = Console.ReadLine();

                // Clear last inserted line to prevent message content duplicates in console
                ClearPreviousConsoleLine();

                // Send newly typed chat message to server
                client.ServerContext.Send(RouteNames.SendMessage, new SendMessageDto { Value = newMessage });
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