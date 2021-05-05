using Msh.EasyRabbitMQ.ServiceBus;
using RabbitMQ.Client;

namespace Msh.EasyRabbitMQ.Extensions
{
    public record QueueResult(
        IPublishManager PublishManager,
        PublishOptions PublishOptions);

    public record RouteResult(
        IPublishManager PublishManager,
        PublishOptions PublishOptions);

    public record ExchangeMessageResult(
        IPublishManager PublishManager,
        PublishOptions PublishOptions,
        RouteResult RouteResult);

    public record ExchangeTypeResult(
        IPublishManager PublishManager,
        PublishOptions PublishOptions,
        string Message);

    public record PublishResult(
        ExchangeTypeResult ExchangeTypeResult,
        string Message);

    public record MessageResult(
        QueueResult QueueResult,
        string Message);

    public static class PublishMethods
    {
        public static QueueResult WithQueue(
            this IPublishManager publishManager,
            string queueName) =>
            new(publishManager, new()
            {
                Queue = queueName
            });

        public static MessageResult WithMessage(
            this QueueResult queueResult,
            string message) =>
            new(queueResult, message);

        public static RouteResult WithExchange(
            this IPublishManager publishManager,
            string exchange)
        {
            return new(publishManager, new()
            {
                Exchange = exchange
            });
        }

        public static ExchangeMessageResult WithRoute(
            this RouteResult routeResult,
            string routingKey)
        {
            routeResult.PublishOptions.RoutingKey = routingKey;
            return new(routeResult.PublishManager, routeResult.PublishOptions, routeResult);
        }

        public static ExchangeTypeResult WithMessage(
            this ExchangeMessageResult messageResult,
            string message) =>
            new(messageResult.PublishManager, messageResult.PublishOptions, message);

        public static PublishResult Topic(this ExchangeTypeResult exchangeTypeResult)
        {
            exchangeTypeResult.PublishOptions.ExchangeType = ExchangeType.Topic;
            return new(
                exchangeTypeResult,
                exchangeTypeResult.Message);
        }

        public static PublishResult Direct(this ExchangeTypeResult exchangeTypeResult)
        {
            exchangeTypeResult.PublishOptions.ExchangeType = ExchangeType.Direct;
            return new(
                exchangeTypeResult,
                exchangeTypeResult.Message);
        }

        public static PublishResult Fanout(this ExchangeTypeResult exchangeTypeResult)
        {
            exchangeTypeResult.PublishOptions.ExchangeType = ExchangeType.Fanout;
            return new(
                exchangeTypeResult,
                exchangeTypeResult.Message);
        }
    }
}