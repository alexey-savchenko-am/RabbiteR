namespace Rabbiter.Core.Pipelines.MessageProcessingPipeline
{
    using Rabbiter.Core.Abstractions.Messaging;
    using System;
    using System.Collections.Generic;

    public class MessageWithHandlers
    {
        public MessageWithHandlers(IMessage message, IDictionary<string, Type> handledEventDictionary)
        {
            Message = message;
            HandledEventDictionary = handledEventDictionary;
        }
        public IMessage Message { get; set; }
        public IDictionary<string, Type> HandledEventDictionary { get; set; }

        public Exception MessageProcessingEx { get; set; }
    }


}
