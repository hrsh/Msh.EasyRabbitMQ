using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusConnection;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusOptions;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusPublisher;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusSubscriber;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public static class Installer
    {
        public static IEasyRabbitMqService AddEasyRabbitMq(this IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            services.Configure<RabbitMqOptions>(
                configuration.GetSection(nameof(RabbitMqOptions)),
                opt => configuration.Bind(opt));

            services.AddSingleton<IConnectionManager, ConnectionManager>();

            return new EasyRabbitMqService(services);
        }

        public static IEasyRabbitMqService AddEasyRabbitMqPublisher(
            this IEasyRabbitMqService services)
        {
            services.Services.AddSingleton<IPublishManager, PublishManager>();
            return services;
        }

        public static IEasyRabbitMqService AddEasyRabbitMqSubscriber(
            this IEasyRabbitMqService services)
        {
            services.Services.AddSingleton<ISubscribeManager, SubscribeManager>();
            return services;
        }
    }

    public interface IEasyRabbitMqService
    {
        IServiceCollection Services { get; }
    }

    public class EasyRabbitMqService : IEasyRabbitMqService
    {
        public IServiceCollection Services { get; }

        public EasyRabbitMqService(IServiceCollection services) =>
            Services = services;
    }
}
