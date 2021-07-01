namespace Rabbiter.Core.Abstractions
{
    using RabbitMQ.Client;
    using System;
    using System.Collections.Generic;

    public interface IRmqResourceManager
    {
        IModel OpenChannel(
            IConnection connection, 
            string eventGroupId, 
            IEnumerable<string> eventNames);
    }
}
