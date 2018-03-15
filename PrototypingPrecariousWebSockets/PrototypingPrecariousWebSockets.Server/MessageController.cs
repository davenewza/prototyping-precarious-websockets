using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrototypingPrecariousWebSockets.Server
{
    [Route("api")]
    public class MessageController : Controller
    {
        private IHubContext<Serv> _hubContext;

        public MessageController(IHubContext<Serv> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("send")]
        public string Send(string msg)
        {
            _hubContext.Clients.All.SendAsync("Send", "FROM HTTP");
            return "sent";
        }
    }
}