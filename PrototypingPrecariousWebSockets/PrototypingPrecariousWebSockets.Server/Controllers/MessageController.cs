using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PrototypingPrecariousWebSockets.Server.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrototypingPrecariousWebSockets.Server.Controllers
{
    [Route("api")]
    public class MessageController : Controller
    {
        private IHubContext<ServHub> _hubContext;

        public MessageController(IHubContext<ServHub> hubContext)
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