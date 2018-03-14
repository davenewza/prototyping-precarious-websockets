﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace PrototypingPrecariousWebSockets.Server
{
    public class Serv : Hub
    {
        public Task Send(string message)
        {
            return Clients.All.SendAsync("Send", message);
        }

        public async Task Receive(string message)
        {
            Console.WriteLine("Received: " + message);
            await Send("response");
        }
    }
}