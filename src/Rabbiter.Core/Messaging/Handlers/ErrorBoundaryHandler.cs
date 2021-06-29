namespace Rabbiter.Core.Messaging.Handlers
{
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Handlers;
    using RabbitMQ.Client;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ErrorBoundaryHandler
        : IHandler
    {
        private readonly ILogger<ErrorBoundaryHandler> _logger;
        private readonly MessageConfirmationDispatcher _confirmationDispatcher;
        private readonly IHandler _handler;
      
        public ErrorBoundaryHandler(
            ILogger<ErrorBoundaryHandler> logger,
            MessageConfirmationDispatcher confirmationDispatcher,
            IHandler handler)
        {
            _logger = logger;
            _confirmationDispatcher = confirmationDispatcher;
            _handler = handler;
        }

        public async Task<MessageHandlerDelegate> GetMessageHandlerAsync(IModel channel, IDictionary<string, Type> handledEventDictionary)
        {
            var messageHandler = 
                await _handler.GetMessageHandlerAsync(channel, handledEventDictionary);

            return async message =>
            {
                var id = message.GetRequiredHeader(MessageHeaders.EventId);

                try
                {
                    return await messageHandler
                        .Invoke(message)
                        .ContinueWith(e =>
                        {
                            _confirmationDispatcher.Approve(id);
                            return e.Result;
                        }, TaskContinuationOptions.OnlyOnRanToCompletion);
                }
                catch (Exception)
                {
                    _logger.LogError($"{message} message processing failed");
                    _confirmationDispatcher.Reject(id);
                    throw;
                }

            };

        }

    }
}
