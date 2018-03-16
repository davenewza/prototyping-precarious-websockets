using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Protocols;
using Microsoft.AspNetCore.Sockets;
using System.Net.Http;
using System.Security.Authentication;
using System.Net.WebSockets;
using System.IO;

namespace Test
{
    internal class Program
    {
        private static ArraySegment<byte> AsMessage(string message)
        {
            var encoded = Encoding.UTF8.GetBytes(message);
            // return new ArraySegment<Byte>(encoded, 0, encoded.Length);
            return new ArraySegment<byte>(encoded);
        }

        // https://github.com/aspnet/SignalR/blob/dev/samples/ClientSample/HubSample.cs#L33
        private static void Main(string[] args)
        {
            Console.WriteLine("Socket Client!");

            //var connection = new HubConnectionBuilder()
            //    .WithUrl("http://localhost:5000/user")
            //    .WithConsoleLogger()
            //    .Build();

            var connection2 = new HubConnectionBuilder()
              //  .WithUrl("http://precariouswebsockets.azurewebsites.net/user")
              .WithUrl("http://localhost:5000/user")
                //.WithTransport(Microsoft.AspNetCore.Sockets.TransportType.WebSockets)
                .WithConsoleLogger()
                .Build();

            Task.Delay(2000).Wait();

            connection2.StartAsync().Wait();
            connection2.InvokeAsync("Accept", "dave here");

            try
            {
                var websocket = new ClientWebSocket();
                // websocket.ConnectAsync(new Uri("ws://precariouswebsockets.azurewebsites.net/user"), CancellationToken.None).Wait();
                websocket.ConnectAsync(new Uri("ws://localhost:5000/user"), CancellationToken.None).Wait();

                var negotiation = AsMessage(@"{""protocol"": ""json""}");

                websocket.SendAsync(negotiation, WebSocketMessageType.Text, true, CancellationToken.None).Wait();

                websocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait();
                //'Id' = 42,
                var message = @"{
                    'type': 1,
                    'target': 'Accept',
                    'arguments': [
                        'soookots'
                    ]
                }";
                //AsMessage("{ 'Target' = 'Accept', 'Arguments' = [ 'sookooots' ] }");

                websocket.SendAsync(AsMessage(message), WebSocketMessageType.Text, true, CancellationToken.None).Wait();

                //  Task.WhenAll(Receive(websocket), Send(websocket));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }

            // ConnectToSignalRService().Wait();

            //connection.On<string>("Send", data =>
            //{
            //    Console.WriteLine($"Received: {data}");
            //});

            //connection.Closed += Connection_Closed;

            //connection.StartAsync().Wait();

            //connection.InvokeAsync("Send", "Hello").Wait();

            //while (true)
            //{
            //    Task.Delay(2000).Wait();
            //    connection.SendAsync("Receive", "12,55");
            //}

            Console.ReadLine();

            //string baseUrl = "http://precariouswebsockets.azurewebsites.net/user";
            //var connection = new HttpConnection(new Uri(baseUrl));
            //try
            //{
            //    var closeTcs = new TaskCompletionSource<object>();
            //    connection.Closed += e => closeTcs.SetResult(null);
            //    connection.OnReceived(data => Console.Out.WriteLineAsync($"{Encoding.UTF8.GetString(data)}"));
            //    connection.StartAsync().Wait();

            //    Console.WriteLine($"Connected to {baseUrl}");
            //    var cts = new CancellationTokenSource();
            //    Console.CancelKeyPress += async (sender, a) =>
            //    {
            //        a.Cancel = true;
            //        await connection.DisposeAsync();
            //    };

            //    while (!closeTcs.Task.IsCompleted)
            //    {
            //        var line = Task.Run(() => Console.ReadLine(), cts.Token).Result;

            //        if (line == null)
            //        {
            //            break;
            //        }

            //        connection.SendAsync(Encoding.UTF8.GetBytes(line), cts.Token).Wait();
            //    }
            //}
            //catch (AggregateException aex) when (aex.InnerExceptions.All(e => e is OperationCanceledException))
            //{
            //}
            //catch (OperationCanceledException)
            //{
            //}
            //finally
            //{
            //    connection.DisposeAsync().Wait();
            //}
            // return 0;
        }

        private static void Connection_Closed(Exception obj)
        {
            Console.WriteLine("CLOSE");
        }

        public async static Task<bool> ConnectToSignalRService()
        {
            try
            {
                Uri.TryCreate(new Uri("http://precariouswebsockets.azurewebsites.net"), "user", out Uri url);

                HttpClientHandler handler = new HttpClientHandler
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

                Connection.On<string>("GetSignalRConnectionUniqueId",
                uniqueId =>
                {
                    //DO something here
                });

                //Added conditions because the way to ignore/accept self-signed certificate using .NET Framework and using .NET Standard 2.0 it is different

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