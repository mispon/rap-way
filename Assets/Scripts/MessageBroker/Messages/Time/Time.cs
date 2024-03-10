namespace MessageBroker.Messages.Time
{
    public struct DayLeftMessage
    {
        public int Day;
    }

    public struct WeekLeftMessage
    {
        public int Week;
    }

    public struct MonthLeftMessage
    {
        public int Month;
    }

    public struct TimeFreezeMessage
    {
        public bool IsFreezed;
    }

    public struct TimeActionModeMessage
    {
        public bool HasAction;
    }
}