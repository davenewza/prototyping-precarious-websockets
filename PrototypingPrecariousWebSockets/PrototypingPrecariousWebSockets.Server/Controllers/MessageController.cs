using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PrototypingPrecariousWebSockets.Server.Hubs;
using System;
using System.Linq;

namespace PrototypingPrecariousWebSockets.Server.Controllers
{
    [Route("api")]
    public class MessageController : Controller
    {
        private IHubContext<UserHub> _hubContext;

        public MessageController(IHubContext<UserHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet()]
        public string Get()
        {
            return String.Join("\r\n", Messages.All.Select(x => $"{x.Item1}\t{x.Item2}\t{x.Item3}"));
        }

        [HttpGet("send")]
        public void Send([FromQuery] string value)
        {
            _hubContext.Clients.All.SendAsync("Update", value);
        }
    }
}