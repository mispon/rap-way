using System;
using System.Globalization;

namespace RapWay.Domain.Time
{
    public readonly struct GameDate : IComparable<GameDate>, IEquatable<GameDate>
    {
        public GameDate(int year, int month, int day, int hour)
        {
            if (year < 1 || year > 9999)
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month));
            }

            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth)
            {
                throw new ArgumentOutOfRangeException(nameof(day));
            }

            if (hour < 0 || hour > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(hour));
            }

            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
        }

        public int Year { get; }

        public int Month { get; }

        public int Day { get; }

        public int Hour { get; }

        public GameDate AddHours(long hours)
        {
            long ticks = checked(hours * TimeSpan.TicksPerHour);
            DateTime current = ToDateTime();
            long targetTicks = checked(current.Ticks + ticks);
            DateTime target = new(targetTicks, DateTimeKind.Unspecified);
            return new GameDate(target.Year, target.Month, target.Day, target.Hour);
        }

        public int CompareTo(GameDate other)
        {
            return ToDateTime().CompareTo(other.ToDateTime());
        }

        public bool Equals(GameDate other)
        {
            return Year == other.Year &&
                   Month == other.Month &&
                   Day == other.Day &&
                   Hour == other.Hour;
        }

        public override bool Equals(object obj)
        {
            return obj is GameDate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Year;
                hashCode = hashCode * 397 ^ Month;
                hashCode = hashCode * 397 ^ Day;
                hashCode = hashCode * 397 ^ Hour;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:D4}-{1:D2}-{2:D2} {3:D2}:00",
                Year,
                Month,
                Day,
                Hour);
        }

        public static bool operator ==(GameDate left, GameDate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GameDate left, GameDate right)
        {
            return !left.Equals(right);
        }

        private DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, 0, 0, DateTimeKind.Unspecified);
        }
    }
}
