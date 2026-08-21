using System;

namespace RapWay.Domain.Character
{
    public readonly struct BoundedResource : IEquatable<BoundedResource>
    {
        public BoundedResource(int current, int maximum)
        {
            if (maximum <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum));
            }

            if (current < 0 || current > maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(current));
            }

            Current = current;
            Maximum = maximum;
        }

        public int Current { get; }

        public int Maximum { get; }

        public BoundedResource Gain(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            return new BoundedResource(Math.Min(checked(Current + amount), Maximum), Maximum);
        }

        public BoundedResource Spend(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            return new BoundedResource(Math.Max(Current - amount, 0), Maximum);
        }

        public bool Equals(BoundedResource other)
        {
            return Current == other.Current && Maximum == other.Maximum;
        }

        public override bool Equals(object obj)
        {
            return obj is BoundedResource other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Current, Maximum);
        }
    }
}
