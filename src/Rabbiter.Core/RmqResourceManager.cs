namespace Rabbiter.Core
{
    using Rabbiter.Core.Abstractions;
    using RabbitMQ.Client;
    using System.Collections.Generic;

    public class RmqResourceManager
        : IRmqResourceManager
    {
        private readonly IRmqExchangeFactory _rmqExchangeFactory;
        private readonly IRmqQueueFactory _rmqQueueFactory;

        public RmqResourceManager(
            IRmqExchangeFactory rmqExchangeFactory,
            IRmqQueueFactory rmqQueueFactory)
        {
            _rmqExchangeFactory = rmqExchangeFactory;
            _rmqQueueFactory = rmqQueueFactory;
        }

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
            _rmqExchangeFactory.DeclareExchange(channel, eventGroupId);

            // declare basic queue for basic exchange
            _rmqQueueFactory.DeclareQueue(channel, eventGroupId, eventGroupId);


            // declare own exchange for each event and bind with basic exchange
            foreach (var iEventName in eventNames)
            {
                _rmqExchangeFactory.DeclareExchange(
                    channel,
                    iEventName,
                    ExchangeType.Fanout,
                    eventGroupId
                );
            }

            return channel;

        }
    }
}
