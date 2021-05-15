using System.Collections.Generic;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public interface IRpcManager
    {
        void Outbox(
            string payload,
            string queue = null,
            string correlationId = null,
            string replyTo = null,
            Dictionary<string, object> arguments = null);
    }
}
