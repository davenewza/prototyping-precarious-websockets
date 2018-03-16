using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PrototypingPrecariousWebSockets.Server.Hubs;
using System;

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

        [HttpGet()]
        public string Get()
        {
            return String.Join(" | ", Messages.All);
        }

        //[HttpGet("connections")]
        //public string Get()
        //{
        //    return _hubContext.
        //}

        [HttpGet("send")]
        public string Send()
        {
            _hubContext.Clients.All.SendAsync("Send", "FROM HTTP");
            return "sent";
        }
    }
}