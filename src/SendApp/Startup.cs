using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Msh.EasyRabbitMQ.ServiceBus;

namespace SendApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BusPublisherApp",
                    Version = "v1"
                });
            });
            services.AddEasyRabbitMq();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "BusPublisherApp");
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(e =>
            {
                e.MapGet("/", async context => await context.Response.WriteAsync("BusPublisherApp"));
                e.MapControllers();
            });
        }
    }
}
