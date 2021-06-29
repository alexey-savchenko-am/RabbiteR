using System.Threading.Tasks;

namespace Rabbiter.Core.Abstractions.Events
{

    public interface IEventHandler
    {
    }

    public interface IEventHandler<in TEvent>
        : IEventHandler
        where TEvent : IEvent
    {
        Task HandleAsync(IEventContainer<TEvent> @event);
    }
}
