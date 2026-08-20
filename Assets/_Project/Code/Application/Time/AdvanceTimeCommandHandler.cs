using RapWay.Application.Commands;
using RapWay.Domain.Events;
using RapWay.Domain.State;
using RapWay.Domain.Time;

namespace RapWay.Application.Time
{
    public sealed class AdvanceTimeCommandHandler : ICommandHandler<AdvanceTimeCommand>
    {
        public CommandExecution Execute(GameState state, AdvanceTimeCommand command)
        {
            if (command.Hours <= 0)
            {
                return CommandExecution.Rejected(TimeCommandFailures.InvalidDuration);
            }

            if (!state.Calendar.CanAdvance(command.Hours))
            {
                return CommandExecution.Rejected(TimeCommandFailures.CalendarLimitExceeded);
            }

            GameDate previousDate = state.Calendar.CurrentDate;
            state.AdvanceTime(command.Hours);
            GameDate currentDate = state.Calendar.CurrentDate;

            return CommandExecution.Succeeded(new TimeAdvanced(previousDate, currentDate, command.Hours));
        }
    }
}
