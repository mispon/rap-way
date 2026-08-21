using RapWay.Application.Commands;
using RapWay.Domain.Common;

namespace RapWay.Application.Activities
{
    public static class ActivityCommandFailures
    {
        public static CommandFailure InvalidDefinitionId { get; } =
            new(StableId.Create("activities.invalid_definition_id"));

        public static CommandFailure DefinitionNotFound { get; } =
            new(StableId.Create("activities.definition_not_found"));

        public static CommandFailure InvalidDuration { get; } =
            new(StableId.Create("activities.invalid_duration"));

        public static CommandFailure ActivityAlreadyActive { get; } =
            new(StableId.Create("activities.already_active"));

        public static CommandFailure NoActiveActivity { get; } =
            new(StableId.Create("activities.no_active_activity"));

        public static CommandFailure ActivityIsComplete { get; } =
            new(StableId.Create("activities.already_complete"));

        public static CommandFailure ActivityIsNotComplete { get; } =
            new(StableId.Create("activities.not_complete"));

        public static CommandFailure CannotInterrupt { get; } =
            new(StableId.Create("activities.cannot_interrupt"));

        public static CommandFailure CalendarLimitExceeded { get; } =
            new(StableId.Create("activities.calendar_limit_exceeded"));
    }
}
