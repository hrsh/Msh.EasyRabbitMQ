# Msh.EasyRabbitMQ

a fast and lightweight library to work with RabbitMQ

Install

`Install-Package EasyRabbitMQ`

Usage:

In `Startup.cs` class:

```csharp
using Msh.EasyRabbitMQ.ServiceBus;
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddEasyRabbitMq();
    ...
}
```

In `appsettings.json`

```json
"RabbitMqOptions": {
  "Host": "localhost",
  "Port": "5672",
  "Protocol": "amqp",
  "Username": "guest",
  "Password": "guest",
  "AutoAcknowledge": true,
  "UpdateIntervals": 50,
  "Durable": false,
  "Exclusive": false,
  "PrefetchCount": 1,
  "PrefetchSize": 0,
  "RetryCount": 5,
  "PublishOptions": {
    "Exchange": "report_exchange",
    "RoutingKey": "report.*",
    "ExchangeType": "topic",
    "Queue": "my_queue",
    "Persistent": true
  },
  "SubscribOptions": {
    "Exchange": "report_exchange",
    "Queue": "my_queue",
    "RoutingKey": "report.*",
    "ExchangeType": "topic",
    "TimeToLive": 30000
  }
}
```

Publish a message:

```csharp
private readonly IPublishManager _publishManager;

public SendController(IPublishManager publishManager)
{
    _publishManager = publishManager;
}
...
public IActionResult PublishAction()
{
    _publishManager.PublishUsingQueue(
    /*
        if queue name is null, EasyRabbitMQ uses value in appsettings.json
    */
    );
    return Ok();
}
```

Subscribe to bus:

1. In `Startup.cs`

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddEasyRabbitMq();
    services.AddHostedService<TaskRunner>();
}
```

2. `TaskRunner.cs`

```csharp
public class TaskRunner : IHostedService
{
    private readonly ISubscribeManager _subscribeManager;
    private readonly ILogger<TaskRunner> _logger;

    public TaskRunner(ISubscribeManager subscribeManager, ILogger<TaskRunner> logger)
    {
        _subscribeManager = subscribeManager;
        _logger = logger;

    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // queue or exchange name must match the smae as 
        // publisher
        _subscribeManager.SubscribeUsingQueue();

    // with callback function
    _subscribeManager.SubscribeUsingQueue(Process);

        return Task.CompletedTask;
    }

    Task<bool> Process(string source)
    {
	    // do your work
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```
