namespace Rabbiter.Core.Abstractions.Publishers
{
    using Rabbiter.Core.Abstractions.Events;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;

        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent;

        public void Publish<TEvent>(TEvent @event, string eventGroupId) where TEvent : IEvent;

        public void Publish<TEvent>(TEvent @event, string eventGroupId, string id) where TEvent : IEvent;

        Task PublishFaultAsync<TEvent>(TEvent @event, string errorReason) where TEvent : IEvent;

        public void PublishFault<TEvent>(TEvent @event, string errorReason) where TEvent : IEvent;

        public void PublishFault<TEvent>(TEvent @event, string eventGroupId, string errorReason) where TEvent : IEvent;

        public void PublishFault<TEvent>(TEvent @event, string eventGroupId, string id, string errorReason) where TEvent : IEvent;

        void PublishRange<TEvent>(IEnumerable<TEvent> events, string eventGroupId = "") where TEvent : IEvent;
    }
}
