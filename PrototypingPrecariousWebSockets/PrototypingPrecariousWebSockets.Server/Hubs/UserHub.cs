using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace PrototypingPrecariousWebSockets.Server.Hubs
{
    public class UserHub : Hub
    {
        public UserHub(IHubContext<UserHub> userHub)
        {
            //userHub.
        }

        public void Accept(string message)
        {
            Console.WriteLine("Accept() from Context.ConnectionId");
            Messages.All.Add(message);
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("OnConnectedAsync() - " + Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("OnDisconnectedAsync() - " + Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}