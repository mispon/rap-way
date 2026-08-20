using RapWay.Application.Commands;

namespace RapWay.Application.Time
{
    public sealed class AdvanceTimeCommand : IGameCommand
    {
        public AdvanceTimeCommand(int hours)
        {
            Hours = hours;
        }

        public int Hours { get; }
    }
}
