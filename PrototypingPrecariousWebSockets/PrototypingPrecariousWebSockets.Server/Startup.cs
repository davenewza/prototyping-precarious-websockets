using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrototypingPrecariousWebSockets.Server.Hubs;

namespace PrototypingPrecariousWebSockets
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder => { builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin(); }));
            services.AddSignalR();
            services.AddMvcCore();
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddAzureWebAppDiagnostics().AddDebug();

            app.UseCors("CorsPolicy");

            app.UseSignalR(routes =>
            {
                routes.MapHub<UserHub>("/user");
            });

            app.UseMvc();
        }
    }
}