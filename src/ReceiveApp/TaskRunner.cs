using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Msh.EasyRabbitMQ.ServiceBus;
using Newtonsoft.Json;
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
            _logger.LogInformation("Register subscribtion...");

            //_subscribeManager.SubscribeUsingQueue(ProcessMessage<FakeClass> ,queue: Consts.QueueName);
            //_subscribeManager.SubscribeUsingQueue(queue: Consts.QueueName);
            //_subscribeManager.SubscribeUsingTaskQueue<FakeClass>(
            //    ProcessMessage<FakeClass>, Consts.QueueName);
            _subscribeManager.SubscribeUsingExchange(
                Consts.ExchangeName,
                Consts.RoutingKey,
                Consts.QueueName,
                "topic",
                null);

            _logger.LogInformation("Subscribtion registered.");
            return Task.CompletedTask;
        }

        Task<bool> ProcessMessage<T>(string source)
        {
            var t = JsonConvert.DeserializeObject<T>(source);
            var u = JsonConvert.SerializeObject(t);
            _logger.LogInformation(u);
            return Task.FromResult(true);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
