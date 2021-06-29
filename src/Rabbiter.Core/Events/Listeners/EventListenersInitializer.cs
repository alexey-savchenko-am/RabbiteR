namespace Rabbiter.Core.Events.Listeners
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    class EventListenersInitializer
        : IHostedService
    {
        private readonly ILogger<EventListenersInitializer> _logger;
        private readonly IEnumerable<EventListener> _listeners;

        public EventListenersInitializer(
            ILogger<EventListenersInitializer> logger,
            IEnumerable<EventListener> listeners)
        {
            _logger = logger;
            _listeners = listeners;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var listener in _listeners)
            {
                var id = Guid.NewGuid().ToString();

                if (await listener.StartListeningAsync(id))
                    _logger.LogInformation($"Event listener {id} for event group {listener.EventGroup} successfully registered");
                else
                    _logger.LogInformation($"Can not register event listener {id} for event group {listener.EventGroup}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
