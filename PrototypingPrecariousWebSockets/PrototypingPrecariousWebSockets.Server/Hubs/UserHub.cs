using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PrototypingPrecariousWebSockets.Server.Hubs
{
    public class UserHub : Hub
    {
        private ILogger<UserHub> _logger;

        public UserHub(ILogger<UserHub> logger)
        {
            _logger = logger;
        }

        public bool Accept(string message)
        {
            _logger.LogInformation($"Accept({message}) from {Context.ConnectionId}");

            Messages.All.Add(new Tuple<DateTime, string, string>(DateTime.Now, Context.ConnectionId, $"Accept({message})"));

            this.Clients.Client(Context.ConnectionId).SendAsync("Update", "received msg - ty");

            return true;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"OnConnectedAsync() - {Context.ConnectionId}");

            Messages.All.Add(new Tuple<DateTime, string, string>(DateTime.Now, Context.ConnectionId, $"Connected"));
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var exceptionString = exception != null ? exception.GetType().Name + " - " + exception.Message : String.Empty;

            _logger.LogInformation($"OnDisconnectedAsync() - {Context.ConnectionId} {exceptionString}");

            Messages.All.Add(new Tuple<DateTime, string, string>(DateTime.Now, Context.ConnectionId, $"Disconnected"));
            return base.OnDisconnectedAsync(exception);
        }
    }
}