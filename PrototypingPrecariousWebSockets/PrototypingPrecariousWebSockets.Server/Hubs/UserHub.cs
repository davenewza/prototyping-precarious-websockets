using Microsoft.AspNetCore.SignalR;

namespace PrototypingPrecariousWebSockets.Server.Hubs
{
    public class UserHub : Hub
    {
        public void Accept(string message)
        {
            Messages.All.Add(message);
        }
    }
}