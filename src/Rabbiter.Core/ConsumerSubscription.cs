namespace Rabbiter.Core
{
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Messaging.Consumers;
    using System;

    public class ConsumerSubscription
        : IConsumerSubscription
    {
        public string ConsumerTag { get; }
        public RmqMessageConsumer Consumer { get; }

        private readonly Action _unsubscribe;
		public ConsumerSubscription(string consumerTag, RmqMessageConsumer consumer, Action unsubscribe)
		{
            ConsumerTag = consumerTag;
            Consumer = consumer;
            _unsubscribe = unsubscribe;
		}

		public void Unsubscribe()
		{
			_unsubscribe();
		}
	}
}
