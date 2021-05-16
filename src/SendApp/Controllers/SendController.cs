using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Msh.EasyRabbitMQ.Extensions;
using Msh.EasyRabbitMQ.ServiceBus;
using Msh.EasyRabbitMQ.ServiceBus.ServiceBusPublisher;

namespace SendApp.Controllers
{
    [ApiController, Route("[controller]")]
    public class SendController : ControllerBase
    {
        private readonly IPublishManager _publishManager;
        private readonly IRpcManager _rpcManager;
        private readonly ILogger<SendController> _logger;

        public SendController(
            IPublishManager publishManager,
            IRpcManager rpcManager,
            ILogger<SendController> logger)
        {
            _publishManager = publishManager;
            _rpcManager = rpcManager;
            _logger = logger;
        }

        [HttpGet("{message}")]
        public IActionResult PublishAction(string message)
        {
            //_publishManager.PublishUsingQueue(message, "queue_name");
            _rpcManager.Outbox(
                payload: message,
                queue: "new_queue",
                correlationId: "2e6ed2f5-462f-442e-9b05-8200026f9c61",
                replyTo: "new_reply_queue",
                arguments: null);

            //_publishManager.PublishUsingExchange(
            //    message,
            //    exchange: "my_exchange",
            //    routingKey: "my_key",
            //    exchangeType: "topic",
            //    arguments: null);

            //_publishManager
            //    .WithQueue("queue_name")
            //    .WithMessage(message)
            //    .Publish();




            //_publishManager
            //    .WithExchange("fluent_exchange_name")
            //    .WithRoute("fluent_routing_key")
            //    .WithMessage("fluent_message")
            //    .Topic()
            //    .Publish();

            //_publishManager
            //    .WithExchange("")
            //    .WithRoute("")
            //    .WithMessage("")
            //    .Direct()
            //    .Publish();

            return Ok();
        }

        [HttpGet("rpc/{number}")]
        public IActionResult RpcService(string number)
        {
            //_logger.LogInformation("Send RPC request...");
            //_rpcClient.Open("2e6ed2f5-462f-442e-9b05-8200026f9c61");
            //var r = _rpcClient.Invoke(number, "rpc_queue");
            //_rpcClient.Close();
            //_logger.LogWarning(r);
            return Ok();
        }
    }
}
