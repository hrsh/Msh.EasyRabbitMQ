using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Msh.EasyRabbitMQ.ServiceBus;
using System.Threading;
using System.Threading.Tasks;

namespace ReceiveApp
{
    public class TaskRunner : IHostedService
    {
        private readonly ISubscribeManager _subscribeManager;
        private readonly ILogger<TaskRunner> _logger;

        public TaskRunner(ISubscribeManager subscribeManager, ILogger<TaskRunner> logger)
        {
            _subscribeManager = subscribeManager;
            _logger = logger;

        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //_subscribeManager.SubscribeUsingQueue(Consts.QueueName);

            _subscribeManager.SubscribeUsingExchange(a =>
            {
                a.Exchange = "my_exchange";
                a.ExchangeType = "topic";
                a.RoutingKey = "my_key";
                a.ExchangeType = "topic";
                a.Queue = "my_queue";
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
