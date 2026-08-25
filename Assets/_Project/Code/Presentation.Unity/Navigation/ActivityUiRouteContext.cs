#nullable enable

using System;

namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class ActivityUiRouteContext : IUiRouteContext
    {
        public ActivityUiRouteContext(string activityId, string? sessionId = null, int durationHours = 0)
        {
            if (string.IsNullOrWhiteSpace(activityId))
            {
                throw new ArgumentException("An activity ID is required.", nameof(activityId));
            }

            if (durationHours < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(durationHours), durationHours, "Duration cannot be negative.");
            }

            ActivityId = activityId;
            SessionId = sessionId;
            DurationHours = durationHours;
        }

        public string ActivityId { get; }

        public string? SessionId { get; }

        public int DurationHours { get; }
    }
}
