﻿namespace Rabbiter.Core.Events.Listeners
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Events;

    public class EventListener
    {
        private readonly ILogger<EventListener> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEventConsumer _consumer;
        private readonly EventProcessorGroup _processors;
        private readonly IEventProcessingFaultResolvingStrategy _eventProcessingFaultResolver;
        private IConsumerSubscription _consumerSubscription;

        public bool IsStarted
            => _consumerSubscription != null;

        public string EventGroup { get => _processors.EventGroup; }

        public string CurrentListenerId { get; private set; }

        public EventListener(
            ILogger<EventListener> logger,
            IServiceScopeFactory serviceScopeFactory,
            IEventConsumer consumer,
            EventProcessorGroup processors,
            IEventProcessingFaultResolvingStrategy eventProcessingFaultResolver)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _consumer = consumer;
            _processors = processors;
            _eventProcessingFaultResolver = eventProcessingFaultResolver;
            _consumerSubscription = null;
        }

        public async Task<bool> StartListeningAsync(string listenerId)
        {
            if (_consumerSubscription != null) return true;

            try
            {
                CurrentListenerId = listenerId;

                _consumerSubscription = await _consumer.SubscribeAsync(
                    listenerId,
                    _processors.EventGroup,
                    _processors.ToHandledEventDictionary(),
                    ProcessIncomingEventAsync,
                    ReconnectAsync
                );

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while subscription to event consumer for listener {listenerId}: {ex.Message}");

                if (_consumerSubscription != null)
                {
                    _consumerSubscription.Unsubscribe();
                    _consumerSubscription = null;
                }

                return false;
            }

            return true;
        }

        private async Task<bool> ReconnectAsync(string listenerId = "")
        {
            if (IsStarted) 
            {
                _logger.LogInformation($"Trying to stop listener {CurrentListenerId}");
                StopListening();
            } 

            var lid = string.IsNullOrEmpty(listenerId)
                ? string.IsNullOrEmpty(CurrentListenerId)
                    ? throw new ArgumentNullException("Can't reconnect, unknown listener id")
                    : CurrentListenerId
                : listenerId;

            _logger.LogInformation($"About to RESTART listener id {lid} ...");

            var res = await Task.Run(() => StartListeningAsync(lid));

            if (res) _logger.LogInformation($"Listener {lid} has been successfully RESTARTED");
                else _logger.LogError($"FAILED TO RESTART listener {lid}");

            return res;
        }

  

        private async Task ProcessIncomingEventAsync(IEventContainer<IEvent> @event)
        {
            _logger.LogInformation($"{@event} about to start event processing...");

            bool isFaultedEvent = !string.IsNullOrEmpty(@event.Error);

            var eventProcessors =
                _processors.ResolveEventProcessors(@event.EventType, @event.EventGroupId, isFaultedEvent);

            if (!eventProcessors.Any())
            {
                _logger.LogError($"{@event} handler for event id {@event.Id} not found");
                return;
            }

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {

                var processorTasks = eventProcessors.Select(
                    p => Task.Run(
                        () => p.ProcessEventAsync(@event, scope.ServiceProvider)
                 ));

                var eventProcessorTasks = Task.WhenAll(processorTasks);

                try
                {
                    await eventProcessorTasks;
                }
                catch (Exception)
                {
                    if(eventProcessorTasks.Exception != null)
                    {
                        // If exception occurs within event processor,
                        // don't return NACK to bus, but just absorb the exception!
                        // Futher actions depend on concrete strategy!
                        await _eventProcessingFaultResolver.ResolveFaultAsync(
                            @event, GetBottomException(eventProcessorTasks.Exception).Message);
                    }

                    _logger.LogInformation($"{@event} not processed due an exception");
                    return;
                }

            }

            _logger.LogInformation($"{@event} successfully processed");
        }

        private Exception GetBottomException(Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return GetBottomException(ex.InnerException);
        }


        public void StopListening()
        {
            if (_consumerSubscription != null)
            {
                _consumerSubscription.Unsubscribe();
                _consumerSubscription = null;
            }

        }

    }
}
