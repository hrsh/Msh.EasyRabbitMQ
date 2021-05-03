using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Msh.EasyRabbitMQ.ServiceBus;

namespace ReceiveApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEasyRabbitMq();
            services.AddHostedService<TaskRunner>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("ReceiveApp");
                });
            });
        }
    }
}
