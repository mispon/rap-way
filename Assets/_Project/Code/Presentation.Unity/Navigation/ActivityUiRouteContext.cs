#nullable enable

using System;

namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class ActivityUiRouteContext : IUiRouteContext
    {
        public ActivityUiRouteContext(string activityId, string? sessionId = null)
        {
            if (string.IsNullOrWhiteSpace(activityId))
            {
                throw new ArgumentException("An activity ID is required.", nameof(activityId));
            }

            ActivityId = activityId;
            SessionId = sessionId;
        }

        public string ActivityId { get; }

        public string? SessionId { get; }
    }
}
