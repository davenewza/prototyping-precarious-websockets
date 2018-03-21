using Microsoft.AspNetCore.SignalR;

namespace PrototypingPrecariousWebSockets.Server.Hubs
{
    public class MonitorHub : Hub
    {
        public MonitorHub()
        { }

        public void Log(string message)
        {
            Clients.All.SendAsync("Log", message);
        }
    }
}