using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        private static void Main(string[] args)
        {
            RunWebSockets().GetAwaiter().GetResult();
        }

        private static async Task RunWebSockets()
        {
            // Refer to Hub Protocol specification when using websockets: https://github.com/aspnet/SignalR/blob/dev/specs/HubProtocol.md
            var websocket = new ClientWebSocket();
            await websocket.ConnectAsync(new Uri("ws://localhost:5000/user"), CancellationToken.None);

            // First, send negotiation message.
            var negotiation = AsSignalRMessage(@"{""protocol"":""json""}");
            await websocket.SendAsync(negotiation, WebSocketMessageType.Text, true, CancellationToken.None);

            if (websocket.State == WebSocketState.CloseReceived)
            {
                throw new InvalidOperationException("Negotiation failed.");
            }

            var sending = Task.Run(async () =>
            {
                string text;
                do
                {
                    text = Console.ReadLine();

                    var messageObj = new { InvocationId = Guid.NewGuid(), Type = 1, Target = "Accept", Arguments = new[] { text } };
                    var message = JsonConvert.SerializeObject(messageObj, Formatting.None, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                    await websocket.SendAsync(AsSignalRMessage(message), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                while (!String.IsNullOrWhiteSpace(text));

                await websocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, String.Empty, CancellationToken.None);
            });

            var receiving = Receiving(websocket);

            await Task.WhenAll(sending, receiving);
        }

        private static async Task Receiving(ClientWebSocket ws)
        {
            var buffer = new byte[2048];

            while (true)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

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
                    await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, String.Empty, CancellationToken.None);
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