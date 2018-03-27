using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;
using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace PrototypingPrecariousWebSockets.SignalRClient
{
    internal class Program
    {
        //  private const string uri = "https://precariouswebsockets.azurewebsites.net/user";
        private const string uri = "http://localhost:5000/user";

        private static void Main(string[] args)
        {
            Console.WriteLine("SignalR Client\n==============\n");

            RunSignalRClient().GetAwaiter().GetResult();

            Console.ReadLine();
        }

        private static async Task RunSignalRClient()
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true,
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            };

            var hubConnection = new HubConnectionBuilder()
                .WithUrl(uri)
                .WithTransport(TransportType.All)
                // .WithWebSocketOptions(options => options.KeepAliveInterval = TimeSpan.FromSeconds(10))
                .WithMessageHandler(handler)
                .Build();

            await hubConnection.StartAsync();

            hubConnection.On<string>("Update", Update);
            hubConnection.On("Confirm", Confirm);
            hubConnection.Closed += HubConnectionClosed;

            var sending = Sending(hubConnection);

            await Task.WhenAll(sending);
        }

        private static Task Sending(HubConnection hubConnection)
        {
            return Task.Run(async () =>
            {
                string text;
                do
                {
                    text = Console.ReadLine();

                    var result = await hubConnection.InvokeAsync<bool>("Accept", text, CancellationToken.None);

                    Console.WriteLine($"Result = {result}");
                }
                while (!String.IsNullOrWhiteSpace(text));

                await hubConnection.StopAsync();
            });
        }

        private static void HubConnectionClosed(Exception exception)
        {
            Console.WriteLine(exception);
        }

        private static void Update(string value)
        {
            Console.WriteLine($"Update({value})");
        }

        private static void Confirm()
        {
            Console.WriteLine($"Confirm()");
        }
    }
}