namespace Rabbiter.Core.Abstractions
{
    using System.Threading.Tasks;

    public interface IMessageConfirmation
    {
        Task ConfirmationTask { get; }

        string MessageId { get; }

        void Confirm();

        void Reject();
    }
}
