using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrototypingPrecariousWebSockets.WebSocketsClient
{
    internal class Program
    {
        //private const string uri = "wss://precariouswebsockets.azurewebsites.net/user";
        private const string uri = "ws://localhost:5000/user";

        private static void Main(string[] args)
        {
            Console.WriteLine("WebSocket Client\n================\n");

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

                     var messageObj = new InvocationMessage()
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

                     try
                     {
                         await webSocket.SendAsync(AsSignalRMessage(messageObj), WebSocketMessageType.Text, true, CancellationToken.None);
                     }
                     catch (Exception exception)
                     {
                         Console.WriteLine(exception);
                     }
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
                    var parsed = Encoding.UTF8.GetString(buffer, 0, result.Count - 1);

                    try
                    {
                        var message = JsonConvert.DeserializeObject<InvocationMessage>(parsed);

                        if (message.Target == "Update")
                        {
                            Accept(message.Arguments[0] as string);
                        }
                    }
                    catch (Exception) { }

                    Console.WriteLine(parsed);
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

        private static void Accept(string value)
        {
            Console.WriteLine($"Accept({value})");
        }

        public class InvocationMessage
        {
            public string InvocationId { get; set; }

            public int Type { get; set; }

            public string Target { get; set; }

            public object[] Arguments { get; set; }
        }
    }
}