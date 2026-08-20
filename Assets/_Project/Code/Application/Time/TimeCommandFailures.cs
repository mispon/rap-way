using RapWay.Application.Commands;
using RapWay.Domain.Common;

namespace RapWay.Application.Time
{
    public static class TimeCommandFailures
    {
        public static CommandFailure InvalidDuration { get; } =
            new(StableId.Create("time.invalid_duration"));

        public static CommandFailure CalendarLimitExceeded { get; } =
            new(StableId.Create("time.calendar_limit_exceeded"));
    }
}
