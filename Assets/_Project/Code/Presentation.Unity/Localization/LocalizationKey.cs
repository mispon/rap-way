using System;
using System.Globalization;

namespace RapWay.Presentation.Unity.Localization
{
    public readonly struct LocalizationKey : IEquatable<LocalizationKey>
    {
        public LocalizationKey(string tableName, string entryKey)
        {
            TableName = tableName ?? string.Empty;
            EntryKey = entryKey ?? string.Empty;
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
                int hashCode = TableName != null ? StringComparer.Ordinal.GetHashCode(TableName) : 0;
                hashCode = (hashCode * 397) ^ (EntryKey != null ? StringComparer.Ordinal.GetHashCode(EntryKey) : 0);
                return hashCode;
            }
        }
    }

    public readonly struct LocalizationArgument
    {
        public LocalizationArgument(string name, object value)
        {
            Name = name ?? string.Empty;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }

        public string GetStringValue()
        {
            return Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}
