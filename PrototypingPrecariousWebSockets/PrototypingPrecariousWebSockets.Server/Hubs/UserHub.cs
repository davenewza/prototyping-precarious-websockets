using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace PrototypingPrecariousWebSockets.Server.Hubs
{
    public class UserHub : Hub
    {
        public UserHub(IHubContext<UserHub> userHub)
        {
        }

        public async Task<bool> Accept(string message)
        {
            Console.WriteLine($"Accept({message}) from {Context.ConnectionId}");
            Messages.All.Add(message);
            await Task.Delay(2000);
            return true;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"OnConnectedAsync() - {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"OnDisconnectedAsync() - {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}