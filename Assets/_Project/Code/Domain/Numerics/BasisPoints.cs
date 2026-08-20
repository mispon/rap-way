using System;

namespace RapWay.Domain.Numerics
{
    public readonly struct BasisPoints : IEquatable<BasisPoints>
    {
        public const int OneHundredPercent = 10_000;
        public const int MinimumValue = -1_000_000;
        public const int MaximumValue = 1_000_000;

        private BasisPoints(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public static BasisPoints One => new(OneHundredPercent);

        public static BasisPoints From(int value)
        {
            if (value < MinimumValue || value > MaximumValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return new BasisPoints(value);
        }

        public bool Equals(BasisPoints other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is BasisPoints other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(BasisPoints left, BasisPoints right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BasisPoints left, BasisPoints right)
        {
            return !left.Equals(right);
        }
    }
}
