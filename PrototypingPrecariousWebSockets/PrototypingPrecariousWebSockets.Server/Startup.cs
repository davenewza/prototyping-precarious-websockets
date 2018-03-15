﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PrototypingPrecariousWebSockets.Server.Hubs;

namespace PrototypingPrecariousWebSockets
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddMvcCore();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSignalR(routes =>
            {
                routes.MapHub<ServHub>("/serv");
                routes.MapHub<UserHub>("/user");
            });

            app.UseMvc();
        }
    }
}