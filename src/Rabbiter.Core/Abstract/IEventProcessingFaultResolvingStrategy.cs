using Rabbiter.Core.Abstractions.Events;
namespace Rabbiter.Core.Abstractions
{
    using System;
    using System.Threading.Tasks;

    public interface IEventProcessingFaultResolvingStrategy
    {
        Task ResolveFaultAsync<TEvent>(IEventContainer<TEvent> @event, Exception exception)
            where TEvent : IEvent;

        Task ResolveFaultAsync<TEvent>(IEventContainer<TEvent> @event, string exception)
            where TEvent : IEvent;
    }
}
