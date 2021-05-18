using System.Threading.Tasks;

namespace RichText.Abstractions
{
    public interface ICommandHandler<TCommand>
    {
        Task HandleAsync(TCommand command);
    }

    public interface ICommandHandlerWithResponse<TCommand, TResponse>
    {
        Task<TResponse?> HandleAsync(TCommand command);
    }
}
