namespace Rabbiter.Core.Pipelines.MessageProcessingPipeline
{
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Pipelines.Builders;
    using System;
    using System.Threading.Tasks;

    public class MessageStorageStep
        : IPipelineStep<MessageWithHandlers, IEventContainer<IEvent>>
    {
        // Add something useful here later
        public Task<IEventContainer<IEvent>> InvokeAsync(MessageWithHandlers input, Func<MessageWithHandlers, Task<IEventContainer<IEvent>>> next)
        {
            return next.Invoke(input);
        }
    }
}
