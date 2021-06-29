namespace Rabbiter.Core.Messaging.Handlers
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Abstractions.Handlers;
    using Rabbiter.Core.Abstractions.Messaging;
    using Rabbiter.Core.Pipelines.Builders;
    using Rabbiter.Core.Pipelines.MessageProcessingPipeline;
    using RabbitMQ.Client;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MessageToEventTransformHandler
         : IHandler
    {
        private readonly ILogger<MessageToEventTransformHandler> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MessageToEventTransformHandler(
            ILogger<MessageToEventTransformHandler> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<MessageHandlerDelegate> GetMessageHandlerAsync(IModel channel, IDictionary<string, Type> handledEventDictionary)
        {
            var messagePipeline =
                PipelineBuilder<MessageWithHandlers, IEventContainer<IEvent>>
                    .StartWith<MessageValidationStep>(_serviceScopeFactory)
                    .AddStep<MessageStorageStep>()
                    .EndWithAndBuild<MessageTransformationStep>();

            return message =>
                    messagePipeline
                        .Invoke(new MessageWithHandlers(message, handledEventDictionary));
        }
    }
}
