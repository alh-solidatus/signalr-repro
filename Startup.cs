// Startup.cs

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace SignalRBugRepro
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                // // Use default timeout values for reproduction
                // options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                // options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                
                // Enable detailed logging
                options.EnableDetailedErrors = true;
            })
            .AddHubOptions<DataHub>(options =>
            {
                // Set maximum message size to accommodate large payloads
                options.MaximumReceiveMessageSize = 102400000; // 100MB
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<DataHub>("/hub");
            });
        }
    }
}
