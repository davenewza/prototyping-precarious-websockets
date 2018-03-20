using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PrototypingPrecariousWebSockets.Server.Hubs
{
    public class UserHub : Hub
    {
        private ILogger<UserHub> _logger;

        public UserHub(IHubContext<UserHub> userHub, ILogger<UserHub> logger)
        {
            _logger = logger;
        }

        public async Task Accept(string message)
        {
            _logger.LogInformation($"Accept({message}) from {Context.ConnectionId}");
            Messages.All.Add(message);
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"OnConnectedAsync() - {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"OnDisconnectedAsync() - {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}