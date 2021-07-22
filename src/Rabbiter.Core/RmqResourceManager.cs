namespace Rabbiter.Core
{
    using Rabbiter.Core.Abstractions;
    using RabbitMQ.Client;
    using System.Collections.Generic;
    using System.Linq;

    public class RmqResourceManager
        : IRmqResourceManager
    {
        //      |------------------------------|-------------|
        //      | Event1    Event2     Event3  |             |   
        //      |     |      /\         /      |  EXCHANGES  |
        //      |   Listener1  Listener2       |             |
        //      |-------|----------|-----------|-------------|
        //      |       |          |           |             |
        //      |       |          |           |   QUEUES    |
        //      |     Queue1     Queue2        |             |
        //      |------------------------------|-------------|
        public IModel OpenChannel(IConnection connection, string eventGroupId, IEnumerable<string> eventNames)
        {

             // each channel has its own RabbitMQ consumer
             var channel = connection.CreateModel();
             
             // declare basic exchange
             channel.DeclareExchange(eventGroupId);
             
             // declare basic queue for basic exchange
             channel.DeclareQueue(eventGroupId, eventGroupId);
             
             var eventNamesList = eventNames.ToList();
             
             // declare own exchange for each event and bind with basic exchange
             foreach (var iEventName in eventNamesList)
             {
                 channel.DeclareExchange(
                     iEventName,
                     ExchangeType.Fanout,
                     eventGroupId
                 );
             }
             
             return channel;
            
        }
    }
}
