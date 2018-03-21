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
        //private const string uri = "ws://precariouswebsockets.azurewebsites.net/user";
        private const string uri = "ws://localhost:5000/user";

        private static void Main(string[] args)
        {
            RunWebSocketsClient().GetAwaiter().GetResult();

            Console.ReadLine();
        }

        private static async Task RunWebSocketsClient()
        {
            // Refer to Hub Protocol specification when using websockets: https://github.com/aspnet/SignalR/blob/dev/specs/HubProtocol.md
            var webSocket = new ClientWebSocket();

            await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);

            var negotiationObj = new
            {
                Protocol = "json"
            };

            // First, send negotiation message.
            await webSocket.SendAsync(AsSignalRMessage(negotiationObj), WebSocketMessageType.Text, true, CancellationToken.None);

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
                         // InvocationId for a blocking request.
                         InvocationId = Guid.NewGuid().ToString(),
                         // Message type - 1 for invocation message.
                         Type = 1,
                         // The target name, as expected by the callee's binder.
                         Target = "Accept",
                         // Array of arguments to apply to the method referred to in target.
                         Arguments = new[] { text }
                     };

                     await webSocket.SendAsync(AsSignalRMessage(messageObj), WebSocketMessageType.Text, true, CancellationToken.None);
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
                    throw new NotImplementedException();
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine($"CloseStatus = {result.CloseStatus}");

                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, String.Empty, CancellationToken.None);
                    break;
                }
            }
        }

        private static ArraySegment<byte> AsSignalRMessage(object messageObj)
        {
            var message = JsonConvert.SerializeObject(
                messageObj,
                Formatting.None,
                new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var encoded = Encoding.UTF8.GetBytes(message);
            var bytes = new ArraySegment<byte>(encoded);

            // Append 0x1e to denote end of message for SignalR server.
            return bytes.Append((byte)0x1e).ToArray();
        }
    }
}