using NUnit.Framework;
using RapWay.Domain.Events;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class DomainExecutionTests
    {
        [Test]
        public void ExistingDomainEventExecutesWithoutUnity()
        {
            TimeEvents.DayPassedEvent domainEvent = new(42);

            Assert.That(domainEvent.TotalDays, Is.EqualTo(42));
        }
    }
}
