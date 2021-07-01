using Rabbiter.Core.Abstractions.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Rabbiter.Core.Abstractions.Messaging
{
    public class HandlerModel
    {
        public IModel Channel { get; set; }
        public IDictionary<string, Type> HandledEventDictionary { get; set; }

        public EventHandlerDelegate EventHandler { get; set; }
    }
}
