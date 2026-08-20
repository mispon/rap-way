using System;

namespace RapWay.Domain.Time
{
    public sealed class CalendarState
    {
        public const long MaximumTotalHours = 5_000_000;

        public CalendarState(GameDate startDate, long totalHours = 0)
        {
            if (totalHours < 0 || totalHours > MaximumTotalHours)
            {
                throw new ArgumentOutOfRangeException(nameof(totalHours));
            }

            StartDate = startDate;
            TotalHours = totalHours;
            _ = CurrentDate;
        }

        public GameDate StartDate { get; }

        public long TotalHours { get; private set; }

        public GameDate CurrentDate => StartDate.AddHours(TotalHours);

        public bool CanAdvance(int hours)
        {
            if (hours <= 0 || TotalHours > MaximumTotalHours - hours)
            {
                return false;
            }

            try
            {
                _ = CurrentDate.AddHours(hours);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        internal void Advance(int hours)
        {
            if (!CanAdvance(hours))
            {
                throw new InvalidOperationException("The calendar cannot advance by the requested number of hours.");
            }

            TotalHours = checked(TotalHours + hours);
        }

        internal CalendarState Copy()
        {
            return new CalendarState(StartDate, TotalHours);
        }

        internal void Validate()
        {
            if (TotalHours < 0 || TotalHours > MaximumTotalHours)
            {
                throw new InvalidOperationException("Calendar hours are outside the authoritative range.");
            }

            _ = CurrentDate;
        }
    }
}
