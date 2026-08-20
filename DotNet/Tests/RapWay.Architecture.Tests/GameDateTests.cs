using NUnit.Framework;
using RapWay.Domain.Time;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class GameDateTests
    {
        [Test]
        public void AddHoursUsesGregorianLeapYears()
        {
            GameDate start = new(2028, 2, 28, 23);

            GameDate result = start.AddHours(25);

            Assert.That(result, Is.EqualTo(new GameDate(2028, 3, 1, 0)));
        }

        [Test]
        public void CalendarDerivesDateFromAuthoritativeTotalHours()
        {
            CalendarState calendar = new(new GameDate(2026, 12, 31, 20), 8);

            Assert.That(calendar.TotalHours, Is.EqualTo(8));
            Assert.That(calendar.CurrentDate, Is.EqualTo(new GameDate(2027, 1, 1, 4)));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void CalendarRejectsNonPositiveAdvancement(int hours)
        {
            CalendarState calendar = new(new GameDate(2026, 1, 1, 0));

            Assert.That(calendar.CanAdvance(hours), Is.False);
        }
    }
}
