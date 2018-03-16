using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        private static ArraySegment<byte> AsMessage(string message)
        {
            var encoded = Encoding.UTF8.GetBytes(message);
            var bytes = new ArraySegment<byte>(encoded);
            return bytes.Append((byte)0x1e).ToArray();
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Socket Client!");

            string url = "http://precariouswebsockets.azurewebsites.net/user"; //http://localhost:5000/user

            var signalRConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithConsoleLogger()
                .Build();

            signalRConnection.StartAsync().Wait();
            signalRConnection.InvokeAsync("Accept", "SignalR Client: Test Message!");

            // Refer to Hub Protocol specification when using websockets: https://github.com/aspnet/SignalR/blob/dev/specs/HubProtocol.md
            var websocket = new ClientWebSocket();
            websocket.ConnectAsync(new Uri("ws://localhost:5000/user"), CancellationToken.None).Wait();

            var negotiation = AsMessage(@"{""protocol"":""json""}");
            websocket.SendAsync(negotiation, WebSocketMessageType.Text, true, CancellationToken.None).Wait();

            var message = @"{
                ""type"": 1,
                ""target"": ""Accept"",
                ""arguments"": [
                    ""soookots""
                ]
            }";

            websocket.SendAsync(AsMessage(message), WebSocketMessageType.Text, true, CancellationToken.None).Wait();

            Console.ReadLine();
        }

        public async static Task<bool> ConnectToSignalRService()
        {
            try
            {
                Uri.TryCreate(new Uri("http://precariouswebsockets.azurewebsites.net"), "user", out Uri url);

                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true,
                    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
                };

                var Connection = new HubConnectionBuilder()
                    .WithUrl(url)
                    .WithTransport(TransportType.WebSockets)
                    .WithMessageHandler(handler)
                    .WithConsoleLogger()
                    .Build();

                Connection.On<string>("GetSignalRConnectionUniqueId", uniqueId => { });

                await Connection.StartAsync();
                await Connection.InvokeAsync("Accept", "WEBSOOOKOOTS");
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
    }
}