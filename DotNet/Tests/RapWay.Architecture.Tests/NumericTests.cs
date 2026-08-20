using System;
using NUnit.Framework;
using RapWay.Domain.Numerics;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class NumericTests
    {
        [TestCase(5, FixedRounding.TowardZero, 2)]
        [TestCase(5, FixedRounding.AwayFromZero, 3)]
        [TestCase(5, FixedRounding.NearestAwayFromZero, 3)]
        [TestCase(-5, FixedRounding.TowardZero, -2)]
        [TestCase(-5, FixedRounding.AwayFromZero, -3)]
        [TestCase(-5, FixedRounding.NearestAwayFromZero, -3)]
        public void FixedMathAppliesExplicitRounding(long value, FixedRounding rounding, long expected)
        {
            long result = FixedMath.MultiplyDivide(value, 1, 2, rounding);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FixedMathRejectsTechnicalOverflow()
        {
            Assert.That(
                (Action)(() => FixedMath.MultiplyDivide(long.MaxValue, long.MaxValue, 1)),
                Throws.TypeOf<OverflowException>());
        }

        [Test]
        public void MoneyScaleUsesMinorUnitsAndBasisPoints()
        {
            Money value = Money.FromMinorUnits(12_345);

            Money result = value.Scale(BasisPoints.From(12_500));

            Assert.That(result.MinorUnits, Is.EqualTo(15_431));
        }

        [Test]
        public void MoneyRejectsGameplayCapOverflow()
        {
            Money maximum = Money.FromMinorUnits(Money.MaximumMinorUnits);

            Assert.That(
                (Action)(() => maximum.Add(Money.FromMinorUnits(1))),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void BasisPointsRejectsOutOfRangeMultiplier()
        {
            Assert.That(
                (Action)(() => BasisPoints.From(BasisPoints.MaximumValue + 1)),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
