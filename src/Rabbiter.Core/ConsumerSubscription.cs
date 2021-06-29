namespace Rabbiter.Core
{
    using Rabbiter.Core.Abstractions;
    using System;

    public class ConsumerSubscription
        : IConsumerSubscription
    {
        public string ConsumerTag { get; }

		private readonly Action _unsubscribe;
		public ConsumerSubscription(string consumerTag, Action unsubscribe)
		{
            ConsumerTag = consumerTag;
            _unsubscribe = unsubscribe;
		}

		public void Unsubscribe()
		{
			_unsubscribe();
		}
	}
}
