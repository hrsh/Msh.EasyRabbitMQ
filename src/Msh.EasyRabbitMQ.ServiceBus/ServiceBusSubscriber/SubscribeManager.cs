using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusConnection;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusOptions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Msh.EasyRabbitMQ.ServiceBus.ServiceBusSubscriber
{
    public class SubscribeManager : ISubscribeManager
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<SubscribeManager> _logger;
        private readonly RabbitMqOptions _options;

        public SubscribeManager(
            IConnectionManager connectionManager,
            IOptions<RabbitMqOptions> options,
            ILogger<SubscribeManager> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
            _options = options.Value;
        }

        public void SubscribeUsingQueue(
            string queue = null,
            Dictionary<string, object> arguments = null)
        {
            _connectionManager.TryConnect();

            var channel = _connectionManager.Channel;

            channel.QueueDeclare(
                queue: queue ?? _options.SubscribOptions.Queue,
                durable: _options.Durable,
                exclusive: _options.Exclusive,
                autoDelete: false,
                arguments: arguments);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, @event) =>
            {
                var body = @event.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
#if DEBUG
                _logger.LogInformation("Received {0}", message);
#endif
            };
            channel.BasicConsume(
                queue: queue ?? _options.SubscribOptions.Queue,
                autoAck: true,
                consumer: consumer);
        }

        public void SubscribeUsingQueue(
            Func<string, Task<bool>> callback,
            string queue = null,
            Dictionary<string, object> arguments = null)
        {
            _connectionManager.TryConnect();

            var channel = _connectionManager.Channel;

            channel.QueueDeclare(
                queue: queue ?? _options.SubscribOptions.Queue,
                durable: _options.Durable,
                exclusive: _options.Exclusive,
                autoDelete: false,
                arguments: arguments);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, @event) =>
            {
                var body = @event.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var result = await callback.Invoke(message);
                if (result)
                    channel.BasicAck(@event.DeliveryTag, true);
#if DEBUG
                _logger.LogInformation("Received {0}", message);
#endif
            };
            channel.BasicConsume(
                queue: queue ?? _options.SubscribOptions.Queue,
                autoAck: false,
                consumer: consumer);
        }

        public void SubscribeUsingQueue<T>(
            Func<string, Task<bool>> callback,
            string queue = null,
            Dictionary<string, object> arguments = null) =>
            SubscribeUsingQueue(callback, queue, arguments);

        public void SubscribeUsingTaskQueue(
            string queue = null,
            Dictionary<string, object> arguments = null)
        {
            _connectionManager.TryConnect();

            var channel = _connectionManager.Channel;

            channel.QueueDeclare(
                queue: queue ?? _options.SubscribOptions.Queue,
                durable: _options.Durable,
                exclusive: _options.Exclusive,
                autoDelete: false,
                arguments: arguments);

            channel.BasicQos(
                prefetchSize: _options.PrefetchSize,
                prefetchCount: _options.PrefetchCount,
                global: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, @event) =>
            {
                var body = @event.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
#if DEBUG
                Console.WriteLine("Received {0}", message);
                Console.WriteLine("Done");
#endif
                channel.BasicAck(deliveryTag: @event.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(
                queue: queue ?? _options.SubscribOptions.Queue,
                autoAck: false,
                consumer: consumer);
        }

        public void SubscribeUsingTaskQueue(
            Func<string, Task<bool>> callback,
            string queue = null,
            Dictionary<string, object> arguments = null)
        {
            _connectionManager.TryConnect();

            var channel = _connectionManager.Channel;

            channel.QueueDeclare(
                queue: queue ?? _options.SubscribOptions.Queue,
                durable: _options.Durable,
                exclusive: _options.Exclusive,
                autoDelete: false,
                arguments: arguments);

            channel.BasicQos(
                prefetchSize: _options.PrefetchSize,
                prefetchCount: _options.PrefetchCount,
                global: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, @event) =>
            {
                var body = @event.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var result = await callback.Invoke(message);
                if (result)
                    channel.BasicAck(@event.DeliveryTag, true);
#if DEBUG
                Console.WriteLine("Received {0}", message);
                Console.WriteLine("Done");
#endif
                channel.BasicAck(deliveryTag: @event.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(
                queue: queue ?? _options.SubscribOptions.Queue,
                autoAck: false,
                consumer: consumer);
        }

        public void SubscribeUsingTaskQueue<T>(
            Func<string, Task<bool>> callback,
            string queue = null,
            Dictionary<string, object> arguments = null) =>
            SubscribeUsingTaskQueue(callback, queue, arguments);

        public void SubscribeUsingExchange(
            string exchange = null,
            string routingKey = null,
            string queue = null,
            string exchangeType = null,
            Dictionary<string, object> arguments = null)
        {
            _connectionManager.TryConnect();

            var channel = _connectionManager.Channel;

            channel.ExchangeDeclare(
                exchange: exchange ?? _options.SubscribOptions.Exchange,
                type: ExchangeType.Topic ?? exchangeType);

            channel.QueueDeclare(
                queue ?? _options.SubscribOptions.Queue,
                durable: _options.Durable,
                exclusive: _options.Exclusive,
                autoDelete: false,
                arguments: arguments);

            channel.QueueBind(
                queue: queue ?? _options.SubscribOptions.Queue,
                exchange: exchange ?? _options.SubscribOptions.Exchange,
                routingKey: routingKey ?? _options.SubscribOptions.RoutingKey);

            channel.BasicQos(
                prefetchSize: _options.PrefetchSize,
                prefetchCount: _options.PrefetchCount,
                global: false);

#if DEBUG
            Console.WriteLine("Waiting ...");
#endif
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, @event) =>
            {
                var body = @event.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
#if DEBUG
                Console.WriteLine("Received {0}", message);
#endif
            };
            channel.BasicConsume(
                queue: queue ?? _options.SubscribOptions.Queue,
                autoAck: true,
                consumer: consumer);
        }

        public void SubscribeUsingExchange(
            Func<string, Task<bool>> callback,
            string exchange = null,
            string routingKey = null,
            string queue = null,
            string exchangeType = null,
            Dictionary<string, object> arguments = null)
        {
            _connectionManager.TryConnect();

            var channel = _connectionManager.Channel;

            channel.ExchangeDeclare(
                exchange: exchange ?? _options.SubscribOptions.Exchange,
                type: ExchangeType.Topic ?? exchangeType);

            channel.QueueDeclare(
                queue ?? _options.SubscribOptions.Queue,
                durable: _options.Durable,
                exclusive: _options.Exclusive,
                autoDelete: false,
                arguments: arguments);

            channel.QueueBind(
                queue: queue ?? _options.SubscribOptions.Queue,
                exchange: exchange ?? _options.SubscribOptions.Exchange,
                routingKey: routingKey ?? _options.SubscribOptions.RoutingKey);

            channel.BasicQos(
                prefetchSize: _options.PrefetchSize,
                prefetchCount: _options.PrefetchCount,
                global: false);

#if DEBUG
            Console.WriteLine("Waiting ...");
#endif
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, @event) =>
            {
                var body = @event.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var result = await callback.Invoke(message);
                if (result)
                    channel.BasicAck(@event.DeliveryTag, true);
#if DEBUG
                Console.WriteLine("Received {0}", message);
#endif
            };
            channel.BasicConsume(
                queue: queue ?? _options.SubscribOptions.Queue,
                autoAck: false,
                consumer: consumer);
        }

        public void SubscribeUsingExchange<T>(
            Func<string, Task<bool>> callback,
            string exchange = null,
            string routingKey = null,
            string queue = null,
            string exchangeType = null,
            Dictionary<string, object> arguments = null) =>
            SubscribeUsingExchange(callback, exchange, routingKey, queue, exchangeType, arguments);

        public void SubscribeUsingExchange(
            Action<SubscribOptions> options,
            Dictionary<string, object> arguments = null)
        {
            _connectionManager.TryConnect();

            SubscribOptions subscribOptions = new();
            options.Invoke(subscribOptions);

            var channel = _connectionManager.Channel;

            channel.ExchangeDeclare(
                exchange: subscribOptions.Exchange ?? _options.SubscribOptions.Exchange,
                type: subscribOptions.ExchangeType);

            channel.QueueDeclare(
                subscribOptions.Queue,
                durable: _options.Durable,
                exclusive: _options.Exclusive,
                autoDelete: false,
                arguments: arguments);

            channel.QueueBind(
                queue: subscribOptions.Queue,
                exchange: subscribOptions.Exchange,
                routingKey: subscribOptions.RoutingKey);

            channel.BasicQos(
                prefetchSize: _options.PrefetchSize,
                prefetchCount: _options.PrefetchCount,
                global: false);

#if DEBUG
            Console.WriteLine("Waiting ...");
#endif
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, @event) =>
            {
                var body = @event.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
#if DEBUG
                Console.WriteLine("Received {0}", message);
#endif
            };
            channel.BasicConsume(
                queue: subscribOptions.Queue,
                autoAck: true,
                consumer: consumer);
        }

        public void SubscribeUsingExchange(
            Action<SubscribOptions> options,
            Func<string, Task<bool>> callback,
            Dictionary<string, object> arguments = null)
        {
            _connectionManager.TryConnect();

            SubscribOptions subscribOptions = new();
            options.Invoke(subscribOptions);

            var channel = _connectionManager.Channel;

            channel.ExchangeDeclare(
                exchange: subscribOptions.Exchange,
                type: subscribOptions.ExchangeType);

            channel.QueueDeclare(
                queue: subscribOptions.Queue,
                durable: _options.Durable,
                exclusive: _options.Exclusive,
                autoDelete: false,
                arguments: arguments);

            channel.QueueBind(
                queue: subscribOptions.Queue,
                exchange: subscribOptions.Exchange,
                routingKey: subscribOptions.RoutingKey);

            channel.BasicQos(
                prefetchSize: _options.PrefetchSize,
                prefetchCount: _options.PrefetchCount,
                global: false);

#if DEBUG
            Console.WriteLine("Waiting ...");
#endif
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, @event) =>
            {
                var body = @event.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var result = await callback.Invoke(message);
                if (result)
                    channel.BasicAck(@event.DeliveryTag, true);
#if DEBUG
                Console.WriteLine("Received {0}", message);
#endif
            };
            channel.BasicConsume(
                queue: subscribOptions.Queue,
                autoAck: false,
                consumer: consumer);
        }

        public void SubscribeUsingExchange<T>(
            Action<SubscribOptions> options,
            Func<string, Task<bool>> callback,
            Dictionary<string, object> arguments = null) =>
            SubscribeUsingExchange(options, callback, arguments);
    }
}