namespace Msh.EasyRabbitMQ.ServiceBus
{
    public class SubscribOptions
    {
        public string Exchange { get; set; }

        public string Queue { get; set; }

        public string RoutingKey { get; set; }

        public string ExchangeType { get; set; }

        public int TimeToLive { get; set; }
    }
}
