using RabbitMQ.Client;
using System;

namespace Msh.EasyRabbitMQ.ServiceBus.ServiceBusConnection
{
    public interface IConnectionManager : IDisposable
    {
        IModel Channel { get; }

        void Connect();

        void TryConnect();
    }
}
