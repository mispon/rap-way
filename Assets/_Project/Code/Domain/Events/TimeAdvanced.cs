using RapWay.Domain.Time;

namespace RapWay.Domain.Events
{
    public sealed class TimeAdvanced : IDomainEvent
    {
        public TimeAdvanced(GameDate previousDate, GameDate currentDate, int elapsedHours)
        {
            PreviousDate = previousDate;
            CurrentDate = currentDate;
            ElapsedHours = elapsedHours;
        }

        public GameDate PreviousDate { get; }

        public GameDate CurrentDate { get; }

        public int ElapsedHours { get; }
    }
}
