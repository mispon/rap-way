using System;

namespace RapWay.Domain.Localization
{
    public readonly struct LocalizationKey : IEquatable<LocalizationKey>
    {
        public LocalizationKey(string tableName, string entryKey)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Localization table name is required.", nameof(tableName));
            }

            if (string.IsNullOrWhiteSpace(entryKey))
            {
                throw new ArgumentException("Localization entry key is required.", nameof(entryKey));
            }

            TableName = tableName;
            EntryKey = entryKey;
        }

        public string TableName { get; }

        public string EntryKey { get; }

        public bool Equals(LocalizationKey other)
        {
            return string.Equals(TableName, other.TableName, StringComparison.Ordinal) &&
                   string.Equals(EntryKey, other.EntryKey, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is LocalizationKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = StringComparer.Ordinal.GetHashCode(TableName);
                return (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(EntryKey);
            }
        }
    }

    public readonly struct LocalizationArgument
    {
        public LocalizationArgument(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Localization argument name is required.", nameof(name));
            }

            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }
    }
}
