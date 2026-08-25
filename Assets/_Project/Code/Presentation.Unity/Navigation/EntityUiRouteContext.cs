using System;

namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class EntityUiRouteContext : IUiRouteContext
    {
        public EntityUiRouteContext(UiEntityType entityType, string entityId)
        {
            if (string.IsNullOrWhiteSpace(entityId))
            {
                throw new ArgumentException("An entity ID is required.", nameof(entityId));
            }

            EntityType = entityType;
            EntityId = entityId;
        }

        public UiEntityType EntityType { get; }

        public string EntityId { get; }
    }
}
