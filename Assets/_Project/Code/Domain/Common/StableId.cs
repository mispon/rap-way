using System;

namespace RapWay.Domain.Common
{
    public readonly struct StableId : IComparable<StableId>, IEquatable<StableId>
    {
        public const int MaxLength = 64;

        private readonly string _value;

        private StableId(string value)
        {
            _value = value;
        }

        public string Value => _value ?? string.Empty;

        public bool IsValid => _value != null;

        public static StableId Create(string value)
        {
            if (!TryCreate(value, out StableId stableId))
            {
                throw new ArgumentException(
                    $"Stable IDs must contain 1-{MaxLength} lowercase ASCII letters, digits, '.', '-', or '_', and must start with a letter or digit.",
                    nameof(value));
            }

            return stableId;
        }

        public static bool TryCreate(string value, out StableId stableId)
        {
            stableId = default;

            if (string.IsNullOrEmpty(value) || value.Length > MaxLength || !IsLetterOrDigit(value[0]))
            {
                return false;
            }

            for (int index = 1; index < value.Length; index++)
            {
                char character = value[index];
                if (!IsLetterOrDigit(character) && character != '.' && character != '-' && character != '_')
                {
                    return false;
                }
            }

            stableId = new StableId(value);
            return true;
        }

        public int CompareTo(StableId other)
        {
            return string.CompareOrdinal(Value, other.Value);
        }

        public bool Equals(StableId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is StableId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(StableId left, StableId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StableId left, StableId right)
        {
            return !left.Equals(right);
        }

        private static bool IsLetterOrDigit(char character)
        {
            return character >= 'a' && character <= 'z' || character >= '0' && character <= '9';
        }
    }
}
