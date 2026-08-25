using System;
using System.Collections.Generic;
using RapWay.Application.Activities;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;
using RapWay.Infrastructure.Activities;

namespace RapWay.Infrastructure.Content
{
    public sealed class GameContentCatalog : IActivityDefinitionCatalog
    {
        private readonly ActivityDefinitionCatalog _activities;

        public GameContentCatalog(ActivityDefinitionCatalog activities)
        {
            _activities = activities ?? throw new ArgumentNullException(nameof(activities));
        }

        public bool TryGet(StableId id, out ActivityDefinition definition)
        {
            return _activities.TryGet(id, out definition);
        }

        public IReadOnlyList<ActivityDefinition> Definitions => _activities.Definitions;
    }
}
