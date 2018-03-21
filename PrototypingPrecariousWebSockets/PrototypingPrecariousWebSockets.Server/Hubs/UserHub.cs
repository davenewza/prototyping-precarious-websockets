using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PrototypingPrecariousWebSockets.Server.Hubs
{
    public class UserHub : Hub
    {
        private ILogger<UserHub> _logger;

        private IHubContext<MonitorHub> _monitorHub;

        public UserHub(IHubContext<UserHub> userHub, IHubContext<MonitorHub> monitorHub, ILogger<UserHub> logger)
        {
            _logger = logger;
        }

        public bool Accept(string message)
        {
            _logger.LogInformation($"Accept({message}) from {Context.ConnectionId}");

            //  Monitor($"Accept({message}) from {Context.ConnectionId}");

            Messages.All.Add(message);
            return true;
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