namespace Msh.EasyRabbitMQ.ServiceBus
{
    public class PublishOptions
    {
        public string Exchange { get; set; }

        public string RoutingKey { get; set; }

        public string ExchangeType { get; set; }

        public string Queue { get; set; }

        public bool Persistent { get; set; }
    }
}
