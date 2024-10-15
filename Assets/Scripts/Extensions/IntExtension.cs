namespace Extensions
{
    public static class IntExtension
    {
        public static string GetDisplay(this int value)
        {
            return $"{value:N0}";
        }

        public static string GetMoney(this int value)
        {
            return $"{value:N0}$";
        }

        public static string GetShort(this int value)
        {
            const int b = 1_000_000_000;
            const int m = 1_000_000;
            const int t = 1_000;

            string valueStr;
            var    suffix = "";

            switch (value)
            {
                case >= b:
                    valueStr = $"{(double) value / b:F1}";
                    suffix   = "B";
                    break;
                case >= m:
                    valueStr = $"{(double) value / m:F1}";
                    suffix   = "M";
                    break;
                case >= t:
                    valueStr = $"{(double) value / t:F1}";
                    suffix   = "T";
                    break;
                default:
                    valueStr = value.ToString();
                    break;
            }

            if (valueStr.EndsWith(".0"))
            {
                valueStr = valueStr[..^2];
            }

            return $"{valueStr}{suffix}";
        }

        public static int GetPercent(this int value, int percent)
        {
            return value / 100 * percent + 1;
        }
    }
}