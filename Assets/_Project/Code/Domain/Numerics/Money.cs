using System;

namespace RapWay.Domain.Numerics
{
    public readonly struct Money : IComparable<Money>, IEquatable<Money>
    {
        public const long MinimumMinorUnits = -9_000_000_000_000_000;
        public const long MaximumMinorUnits = 9_000_000_000_000_000;

        private Money(long minorUnits)
        {
            MinorUnits = minorUnits;
        }

        public long MinorUnits { get; }

        public static Money Zero => new(0);

        public static Money FromMinorUnits(long minorUnits)
        {
            EnsureInRange(minorUnits);
            return new Money(minorUnits);
        }

        public Money Add(Money amount)
        {
            return FromMinorUnits(checked(MinorUnits + amount.MinorUnits));
        }

        public Money Subtract(Money amount)
        {
            return FromMinorUnits(checked(MinorUnits - amount.MinorUnits));
        }

        public Money Scale(BasisPoints multiplier, FixedRounding rounding = FixedRounding.TowardZero)
        {
            long scaled = FixedMath.MultiplyDivide(
                MinorUnits,
                multiplier.Value,
                BasisPoints.OneHundredPercent,
                rounding);
            return FromMinorUnits(scaled);
        }

        public int CompareTo(Money other)
        {
            return MinorUnits.CompareTo(other.MinorUnits);
        }

        public bool Equals(Money other)
        {
            return MinorUnits == other.MinorUnits;
        }

        public override bool Equals(object obj)
        {
            return obj is Money other && Equals(other);
        }

        public override int GetHashCode()
        {
            return MinorUnits.GetHashCode();
        }

        public override string ToString()
        {
            return MinorUnits.ToString();
        }

        public static bool operator ==(Money left, Money right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Money left, Money right)
        {
            return !left.Equals(right);
        }

        private static void EnsureInRange(long minorUnits)
        {
            if (minorUnits < MinimumMinorUnits || minorUnits > MaximumMinorUnits)
            {
                throw new ArgumentOutOfRangeException(nameof(minorUnits), "Money exceeded the authoritative gameplay cap.");
            }
        }
    }
}
