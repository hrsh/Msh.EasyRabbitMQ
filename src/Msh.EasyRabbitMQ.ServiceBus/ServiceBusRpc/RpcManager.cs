using Microsoft.Extensions.Options;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusConnection;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusOptions;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusSubscriber;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public class RpcManager : IRpcManager
    {
        private readonly IConnectionManager _connectionManager;
        private readonly RabbitMqOptions _options;
        private readonly ISubscribeManager _subscribeManager;

        public RpcManager(
            IConnectionManager connectionManager,
            ISubscribeManager subscribeManager,
            IOptions<RabbitMqOptions> options)
        {
            _connectionManager = connectionManager;
            _options = options.Value;
            _subscribeManager = subscribeManager;
        }

        public void Outbox(
            string payload, 
            string queue = null, 
            string correlationId = null, 
            string replyTo = null, 
            Dictionary<string, object> arguments = null)
        {
            _connectionManager.TryConnect();

            var channel = _connectionManager.Channel;

            channel.QueueDeclare(
                queue: queue ?? _options.PublishOptions.Queue,
                durable: _options.Durable,
                exclusive: _options.Exclusive,
                autoDelete: false,
                arguments: arguments);

            var body = Encoding.UTF8.GetBytes(payload);

            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = _options.PublishOptions.Persistent;
            basicProperties.CorrelationId = correlationId ?? Guid.NewGuid().ToString("D");
            basicProperties.ReplyTo = replyTo ?? channel.QueueDeclare().QueueName;

            channel.BasicPublish(
                exchange: "",
                routingKey: queue ?? _options.PublishOptions.Queue,
                basicProperties: basicProperties,
                body: body);

            _subscribeManager.SubscribeUsingQueue(basicProperties.ReplyTo);
        }
    }
}
