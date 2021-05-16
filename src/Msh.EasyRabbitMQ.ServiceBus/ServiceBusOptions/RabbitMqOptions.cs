namespace Msh.EasyRabbitMQ.ServiceBus.ServiceBusOptions
{
    public class RabbitMqOptions
    {
        public string Host { get; set; }

        public string Port { get; set; }

        public string Protocol { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool AutoAcknowledge { get; set; }

        public int UpdateIntervals { get; set; }

        public bool Durable { get; set; }

        public bool Exclusive { get; set; }

        public string ConnectionString =>
            $"{Protocol}://{Username}:{Password}@{Host}:{Port}";

        public PublishOptions PublishOptions { get; set; }

        public SubscribOptions SubscribOptions { get; set; }

        public ushort PrefetchCount { get; set; }

        public uint PrefetchSize { get; set; }

        public int RetryCount { get; set; }
    }
}
