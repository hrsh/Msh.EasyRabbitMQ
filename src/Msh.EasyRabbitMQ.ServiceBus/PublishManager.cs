using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public class PublishManager : IPublishManager
    {
        private readonly IConnectionManager _connectionManager;
        private readonly RabbitMqOptions _options;

        public PublishManager(
            IConnectionManager connectionManager,
            IOptions<RabbitMqOptions> options)
        {
            _connectionManager = connectionManager;
            _options = options.Value;
        }

        public void PublishUsingQueue(
            string message,
            string queue = null,
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

            var body = Encoding.UTF8.GetBytes(message);

            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = _options.PublishOptions.Persistent;

            channel.BasicPublish(
                exchange: "",
                routingKey: queue ?? _options.PublishOptions.Queue,
                basicProperties: basicProperties,
                body: body);
        }

        public void PublishUsingQueue<T>(
            T source,
            string queue = null,
            Dictionary<string, object> arguments = null)
        {
            var message = JsonConvert.SerializeObject(source, Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            PublishUsingQueue(message, queue, arguments);
        }

        public void PublishUsingQueue<T>(
            T source,
            JsonSerializerSettings jsonSerializerSettings,
            string queue = null,
            Dictionary<string, object> arguments = null)
        {
            var message = JsonConvert.SerializeObject(
                source, Formatting.None, 
                jsonSerializerSettings);
            PublishUsingQueue(message, queue, arguments);
        }

        public void PublishUsingTaskQueue(
            string message,
            string queue = null,
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

            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = _options.PublishOptions.Persistent;

            channel.BasicPublish(
                exchange: "",
                routingKey: queue ?? _options.PublishOptions.Queue,
                basicProperties: properties,
                body: body);
        }

        public void PublishUsingTaskQueue<T>(
            T source,
            string queue = null,
            Dictionary<string, object> arguments = null)
        {
            var message = JsonConvert.SerializeObject(source);
            PublishUsingTaskQueue(message, queue, arguments);
        }

        public void PublishUsingExchange(
            string message,
            string exchange = null,
            string routingKey = null,
            string exchangeType = null,
            IDictionary<string, object> arguments = null)
        {
            _connectionManager.TryConnect();

            var channel = _connectionManager.Channel;

            channel.ExchangeDeclare(
                exchange: exchange ?? _options.PublishOptions.Exchange,
                type: ExchangeType.Topic ?? _options.PublishOptions.ExchangeType,
                arguments: arguments);

            var body = Encoding.UTF8.GetBytes(message);

            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = _options.PublishOptions.Persistent;

            channel.BasicPublish(
                exchange: exchange ?? _options.PublishOptions.Exchange,
                routingKey: routingKey ?? _options.PublishOptions.RoutingKey,
                basicProperties: basicProperties,
                body: body);
#if DEBUG
            Console.WriteLine("Sent {0}", message);
#endif
        }

        public void PublishUsingExchange<T>(
            T source,
            string exchange = null,
            string routingKey = null,
            string exchangeType = null,
            IDictionary<string, object> arguments = null)
        {
            var message = JsonConvert.SerializeObject(source);
            PublishUsingExchange(message, exchange, routingKey, exchangeType, arguments);
        }

        public void PublishUsingExchange(
            string message,
            Action<PublishOptions> options,
            IDictionary<string, object> arguments = null)
        {
            PublishOptions publishOptions = new();
            options.Invoke(publishOptions);

            var channel = _connectionManager.Channel;

            channel.ExchangeDeclare(
                exchange: publishOptions.Exchange,
                type: publishOptions.ExchangeType,
                arguments: arguments);

            var body = Encoding.UTF8.GetBytes(message);

            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = publishOptions.Persistent;

            channel.BasicPublish(
                exchange: publishOptions.Exchange,
                routingKey: publishOptions.RoutingKey,
                basicProperties: basicProperties,
                body: body);
#if DEBUG
            Console.WriteLine("Sent {0}", message);
#endif
        }

        public void PublishUsingExchange<T>(
            T source,
            Action<PublishOptions> options,
            IDictionary<string, object> arguments = null)
        {
            var message = JsonConvert.SerializeObject(source);
            PublishUsingExchange(message, options, arguments);
        }
    }
}
