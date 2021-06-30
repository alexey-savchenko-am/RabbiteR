# ![logo](docs/assets/logo-sm.png)
### Event based RabbitMQ client for ASP.NET Core.
Makes it easy to publish messages to RMQ bus, consume messages from bus, handling and resolve possible faults during exchange process.

[![NuGet version (RabbiteR)](https://img.shields.io/nuget/vpre/RabbiteR?color=orange&label=nuget%20package)](https://www.nuget.org/packages/RabbiteR/0.5.0)
## Table of contents
- [Installing](#installing)
- [Getting Started](#getting-started)
    - [Event Definition](#event-definition)
    - [Event Handler Definition](#event-handler-definition)
    - [Transport Registration](#transport-registration)
    - [Setting Event Listener](#setting-event-listener)
    - [Event Publishing](#event-publishing)
- [Resolve Faults](#resolve-faults)
- [Integration Tests](#integration-tests)
- [License](#license)

## Installing
Install NuGet package via NuGet.org

```sh
$ dotnet add package RabbiteR
```
```powershell
PS> Install-Package RabbiteR
```
## Getting Started
### Event Definition

Create an event by implementing IEvent interface.
Set specific name of the event using EventName property.
Rabbiter uses event name as special rmq exchange name.

```csharp
using Rabbiter.Core.Abstractions.Events;
using Rabbiter.Core.Events;
using System.Collections.Generic;

[EventName("order-requested-event")]
public class OrderRequestedEvent
    : IEvent
{
    public IList<Product> Products { get; set; }
}

```
### Event Handler Definition

Create an event handler to process OrderRequestedEvent. 
Implement IEventHandler interface closed by specific event type.
HandleAsync method will be invoked when the event will be received.
It contains contained event as parameter.

```csharp
using Rabbiter.Core.Abstractions.Events;
using System.Linq;
using System.Threading.Tasks;
using WebClient.Event;

public class ReportingOrderRequestedEventHandler
    : IEventHandler<OrderRequestedEvent>
{
    private readonly ILogger<ReportingOrderRequestedEventHandler> _logger;

    public ReportingOrderRequestedEventHandler(ILogger<ReportingOrderRequestedEventHandler> logger)
    {
        _logger = logger;
    }
    public Task HandleAsync(IEventContainer<OrderRequestedEvent> @event)
    {
        _logger.LogInformation($"Requested orders total sum {@event.Event.Products.Sum(p => p.UnitPrice)}$");
        return Task.CompletedTask;
    }
}

```

### Transport Registration
Register rmq transport within Startup class to configure Consumer and Producer for bus.
Configuration builder is used to set up configuration for connecting to rmq endpoint.
You can use RmqConfigurationBuilder for easy configuration settings or just fill RmqConnectionConfiguration object within configBuilder parameter.  

```csharp
services.RegisterRmqTransport(
    () =>
        RmqConfigurationBuilder
            .UseConnectionString(Configuration["RabbitMqConnectionString"])
            .Build(),
    payloadFormat: PayloadFormats.Json
);

```
Use method RegisterEventPublisher ONLY if your service is not configured for event consuming.
In this case only event publisher will be registered.
RmqConfigurationBuilder in the next example allows manually setting for configuration properties.

```csharp
services.RegisterEventPublisher(
    () =>
        RmqConfigurationBuilder
            .UseScheme(Rabbiter.Core.Abstractions.Config.Scheme.Amqp)
            .WithHostAndPort("localhost", 5672)
            .WithCredentials("guest", "guest")
            .WithVirtualHost("/")
            .Build(),
    payloadFormat: PayloadFormats.Json
);

```

### Setting Event Listener

Configure event listener to bind event type with specific event handler.
Parameter eventGroupId will be used as rmq queue name and registered events will be conducted through this queue.

```csharp

services.AddScoped<ReportingOrderRequestedEventHandler>();
services.AddScoped<ReportingOrderAcceptedEventHandler>();

services.RegisterEventListener(
    eventGroupId: "Reporting", // queue name
    options =>
        options
            .SubscribeOn<OrderRequestedEvent, ReportingOrderRequestedEventHandler>()
            .SubscribeOn<OrderAcceptedEvent, ReportingOrderAcceptedEventHandler>()
);

```

### Event Publishing

After transport registration you are able to use IEventPublisher within your services to publish specific event to bus.
```csharp
_eventPublisher.Publish(new OrderAcceptedEvent { Products = products }
```

## Resolve Faults

## Integration Tests

## License
