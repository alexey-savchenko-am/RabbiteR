namespace Rabbiter.Core.Abstractions
{
    using Rabbiter.Core.Abstract.Events;
    using Rabbiter.Core.Abstractions.Events;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEventConsumer
    {
        /// <summary>
        /// Subscribe on events for specific listener
        /// </summary>
        /// <param name="listenerId"></param>
        /// <param name="eventGroupId"></param>
        /// <param name="eventTypes">set of event types which could be processed by this listener</param>
        /// <param name="onCompletion"></param>
        /// <returns></returns>
        Task<IConsumerSubscription> SubscribeAsync(
            string listenerId,
            string eventGroupId,
            IDictionary<string, Type> handledEventDictionary,
            EventHandlerDelegate eventHandler,
            EventListenerReconnectDelegate onReconnect);
    }
}
