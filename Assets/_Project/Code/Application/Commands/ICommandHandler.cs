using RapWay.Domain.State;

namespace RapWay.Application.Commands
{
    public interface ICommandHandler<in TCommand>
        where TCommand : IGameCommand
    {
        CommandExecution Execute(GameState state, TCommand command);
    }
}
