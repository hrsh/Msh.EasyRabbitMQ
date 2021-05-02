using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public static class ServiceBusExtensions
    {
        public static IServiceCollection AddEasyRabbitMq(this IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            services.Configure<RabbitMqOptions>(
                configuration.GetSection(nameof(RabbitMqOptions)),
                opt => configuration.Bind(opt));

            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddSingleton<IPublishManager, PublishManager>();
            services.AddSingleton<ISubscribeManager, SubscribeManager>();

            return services;
        }
    }
}
