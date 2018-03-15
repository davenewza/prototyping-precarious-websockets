using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace PrototypingPrecariousWebSockets.Server.Hubs
{
    public class UserHub : Hub
    {
        private static List<string> _messages = new List<string>();

        public void Accept(string message)
        {
            _messages.Add(message);
        }
    }
}