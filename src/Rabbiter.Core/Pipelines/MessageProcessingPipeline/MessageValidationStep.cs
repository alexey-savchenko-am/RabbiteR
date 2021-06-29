namespace Rabbiter.Core.Pipelines.MessageProcessingPipeline
{
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Abstractions.Publishers;
    using Rabbiter.Core.Pipelines.Builders;
    using System;
    using System.Threading.Tasks;

    public class MessageValidationStep
         : IPipelineStep<MessageWithHandlers, IEventContainer<IEvent>>
    {
        // Add something useful here later
        public Task<IEventContainer<IEvent>> InvokeAsync(MessageWithHandlers input, Func<MessageWithHandlers, Task<IEventContainer<IEvent>>> next)
        {
            try
            {
                return next.Invoke(input);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
