namespace Rabbiter.Core
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Config;
    using Rabbiter.Core.Abstractions.Handlers;
    using Rabbiter.Core.Abstractions.Messaging;
    using Rabbiter.Core.Abstractions.Publishers;
    using Rabbiter.Core.Config;
    using Rabbiter.Core.Events;
    using Rabbiter.Core.Events.Consumers;
    using Rabbiter.Core.Events.Listeners;
    using Rabbiter.Core.Messaging;
    using Rabbiter.Core.Messaging.Handlers;
    using Rabbiter.Core.Messaging.Sender;
    using Rabbiter.Core.Pipelines.MessageProcessingPipeline;
    using Rabbiter.Core.Publishers;
    using RabbitMQ.Client;
    using System;
    using Scrutor;

    public enum PayloadFormats
    {
        Json = 1,
        Xml = 2,
        Csv = 3
    }

    public static class RabbiterServiceCollectionExtensions
    {
        public static void RegisterRmqTransport(this IServiceCollection serviceCollection, 
            Func<RmqConnectionConfiguration> configBuilder, PayloadFormats payloadFormat = PayloadFormats.Json)
        {

            serviceCollection.TryAddSingleton<IRmqResourceManager, RmqResourceManager>();
            serviceCollection.TryAddSingleton<IEventNameResolvingStrategy, EventNameResolver>();
            serviceCollection.TryAddSingleton<MessageConfirmationDispatcher>();
            serviceCollection.RegisterPayloadTransformator(payloadFormat);
            serviceCollection.RegisterPipelineSteps();
            serviceCollection.RegisterMessageHandlers();

            var configuration = configBuilder();

            serviceCollection.TryAddSingleton<IEventConsumer>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                
                 return new EventConsumer(
                    loggerFactory,
                    provider.ConnectToBus(loggerFactory, configuration),
                    provider.GetRequiredService<IRmqResourceManager>(),
                    provider.GetRequiredService<IHandler>()
                );
            });

            serviceCollection.AddHostedService<EventListenersInitializer>();

            RegisterEventPublisher(serviceCollection, configuration, payloadFormat);
        }

        public static void RegisterEventPublisher(this IServiceCollection serviceCollection,
        Func<RmqConnectionConfiguration> configBuilder, PayloadFormats payloadFormat = PayloadFormats.Json)
            => RegisterEventPublisher(serviceCollection, configBuilder(), payloadFormat);

        public static void RegisterEventListener(this IServiceCollection serviceCollection, 
            string eventGroupId,
            Func<EventProcessorBuilder, EventProcessorBuilder> eventProcessorBuilder)
        {
            serviceCollection.TryAddSingleton<IEventProcessingFaultResolvingStrategy, EventProcessingFaultResolver>();
            serviceCollection.AddSingleton(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var consumer = provider.GetRequiredService<IEventConsumer>();
                var eventNameResolver = provider.GetRequiredService<IEventNameResolvingStrategy>();

                var eventProcessorGroup = eventProcessorBuilder( 
                    EventProcessorBuilder.WithEventGroupAndNameResolver(eventGroupId, eventNameResolver)
                ).Build();

                var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

                var eventListener = new EventListener(
                    loggerFactory.CreateLogger<EventListener>(),
                    scopeFactory,
                    consumer,
                    eventProcessorGroup,
                    provider.GetRequiredService<IEventProcessingFaultResolvingStrategy>()
                );

                return eventListener;
            });
        }

        private static void RegisterEventPublisher(this IServiceCollection serviceCollection,
            RmqConnectionConfiguration configuration, PayloadFormats payloadFormat = PayloadFormats.Json)
        {
            serviceCollection.TryAddSingleton<IMessageSender, MessageSender>();
            serviceCollection.RegisterPayloadTransformator(payloadFormat);

            serviceCollection.TryAddSingleton<IEventPublisher>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                return new EventPublisher(
                     loggerFactory.CreateLogger<EventPublisher>(),
                     provider.GetRequiredService<IMessageSender>(),
                     provider.ConnectToBus(loggerFactory, configuration),
                     provider.GetRequiredService<IEventNameResolvingStrategy>(),
                     provider.GetRequiredService<IPayloadTransformStrategy>()
                );
            });
        }

        private static IConnection ConnectToBus(this IServiceProvider provider,
            ILoggerFactory loggerFactory,
            RmqConnectionConfiguration configuration)
        {

            var connectionProvider
                = new RmqConnectionProvider();

            return connectionProvider.Connect(configuration);
        }

        private static void RegisterPayloadTransformator(this IServiceCollection serviceCollection, PayloadFormats payloadFormat)
        {

            switch (payloadFormat)
            {
                case PayloadFormats.Json:
                    serviceCollection.TryAddSingleton<IPayloadTransformStrategy, JsonBasedPayloadTransformator>();
                    return;
                default:
                    throw new NotSupportedException($"payload format {payloadFormat} currently not supported");
            }
        }

        private static void RegisterPipelineSteps(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<MessageValidationStep>();
            serviceCollection.TryAddScoped<MessageStorageStep>();
            serviceCollection.TryAddScoped<MessageTransformationStep>();
        }

        private static void RegisterMessageHandlers(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IHandler, MessageToEventTransformHandler>();
            serviceCollection.Decorate<IHandler, ErrorBoundaryHandler>();
            serviceCollection.Decorate<IHandler, EventTypeBasedHandler>();
        }

    }
}
