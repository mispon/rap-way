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

            return value switch
            {
                > b => $"{value / b}B",
                > m => $"{value / m}M",
                > t => $"{value / t}T",
                _   => value.ToString()
            };
        }

        public static int GetPercent(this int value, int percent)
        {
            return value / 100 * percent + 1;
        }
    }
}