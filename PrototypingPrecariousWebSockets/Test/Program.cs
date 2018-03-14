using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        // https://github.com/aspnet/SignalR/blob/dev/samples/ClientSample/HubSample.cs#L33
        private static void Main(string[] args)
        {
            Console.WriteLine("Socket Client!");

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/serv")
                .WithConsoleLogger()
                .Build();

            connection.On<string>("Send", data =>
            {
                Console.WriteLine($"Received: {data}");
            });

            connection.Closed += Connection_Closed;

            connection.StartAsync().Wait();

            connection.InvokeAsync("Send", "Hello").Wait();

            while (true)
            {
                Task.Delay(2000).Wait();
                connection.SendAsync("Receive", "12,55");
            }
        }

        private static void Connection_Closed(Exception obj)
        {
            Console.WriteLine("CLOSE");
        }
    }
}