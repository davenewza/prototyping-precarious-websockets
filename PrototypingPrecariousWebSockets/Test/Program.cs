using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RunWebSockets().GetAwaiter().GetResult();

            Console.ReadLine();
        }

        private static async Task RunWebSockets()
        {
            // Refer to Hub Protocol specification when using websockets: https://github.com/aspnet/SignalR/blob/dev/specs/HubProtocol.md
            var webSocket = new ClientWebSocket();

            // await webSocket.ConnectAsync(new Uri("ws://localhost:5000/user"), CancellationToken.None);

            await webSocket.ConnectAsync(new Uri("ws://precariouswebsockets.azurewebsites.net/user"), CancellationToken.None);

            //http://precariouswebsockets.azurewebsites.net

            // First, send negotiation message.
            var negotiation = AsSignalRMessage(@"{""protocol"":""json""}");
            await webSocket.SendAsync(negotiation, WebSocketMessageType.Text, true, CancellationToken.None);

            if (webSocket.State == WebSocketState.CloseReceived)
            {
                throw new InvalidOperationException("Negotiation failed.");
            }

            var sending = Sending(webSocket);
            var receiving = Receiving(webSocket);

            await Task.WhenAll(sending, receiving);
        }

        private static Task Sending(ClientWebSocket webSocket)
        {
            return Task.Run(async () =>
             {
                 string text;
                 do
                 {
                     text = Console.ReadLine();

                     var messageObj = new
                     {
                         InvocationId = Guid.NewGuid(), // For a blocking request.
                         Type = 1,
                         Target = "Accept",
                         Arguments = new[] { text }
                     };

                     var message = JsonConvert.SerializeObject(messageObj, Formatting.None, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                     await webSocket.SendAsync(AsSignalRMessage(message), WebSocketMessageType.Text, true, CancellationToken.None);
                 }
                 while (!String.IsNullOrWhiteSpace(text));

                 await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, String.Empty, CancellationToken.None);
             });
        }

        private static async Task Receiving(ClientWebSocket webSocket)
        {
            var buffer = new byte[2048];

            while (true)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    throw new InvalidOperationException("Do not accept binary");
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine($"CloseStatus = {result.CloseStatus}");

                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, String.Empty, CancellationToken.None);
                    break;
                }
            }
        }

        private static ArraySegment<byte> AsSignalRMessage(string message)
        {
            var encoded = Encoding.UTF8.GetBytes(message);
            var bytes = new ArraySegment<byte>(encoded);

            // Append 0x1e to denote end of message for SignalR server.
            return bytes.Append((byte)0x1e).ToArray();
        }
    }
}