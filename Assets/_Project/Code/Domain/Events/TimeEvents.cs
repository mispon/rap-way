using System;

namespace RapWay.Domain.Events
{
    public struct TimeEvents
    {
        public struct DayPassedEvent
        {
            public int TotalDays;
            public DayPassedEvent(int day) => TotalDays = day;
        }
        
        public struct WeekPassedEvent
        {
            public int WeekIndex;
            public WeekPassedEvent(int index) => WeekIndex = index;
        }
        
        public struct MonthPassedEvent
        {
            public int MonthIndex;
            public MonthPassedEvent(int index) => MonthIndex = index;
        }

        public struct YearPassedEvent
        {
            public int YearIndex;
            public YearPassedEvent(int index) => YearIndex = index;
        }
    }
}