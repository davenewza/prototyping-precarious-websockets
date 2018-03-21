using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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