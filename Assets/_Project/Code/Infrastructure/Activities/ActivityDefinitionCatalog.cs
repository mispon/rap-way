using System;
using System.Collections.Generic;
using RapWay.Application.Activities;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;

namespace RapWay.Infrastructure.Activities
{
    public sealed class ActivityDefinitionCatalog : IActivityDefinitionCatalog
    {
        private readonly Dictionary<StableId, ActivityDefinition> _definitions;
        private readonly IReadOnlyList<ActivityDefinition> _orderedDefinitions;

        public ActivityDefinitionCatalog(IReadOnlyList<ActivityDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            _definitions = new Dictionary<StableId, ActivityDefinition>(definitions.Count);
            for (int index = 0; index < definitions.Count; index++)
            {
                ActivityDefinition definition = definitions[index] ??
                                                throw new ArgumentException("Activity definitions cannot contain null.", nameof(definitions));
                if (!_definitions.TryAdd(definition.Id, definition))
                {
                    throw new ArgumentException($"Activity ID '{definition.Id}' is duplicated.", nameof(definitions));
                }
            }

            List<ActivityDefinition> orderedDefinitions = new(_definitions.Values);
            orderedDefinitions.Sort((left, right) => left.Id.CompareTo(right.Id));
            _orderedDefinitions = orderedDefinitions.AsReadOnly();
        }

        public IReadOnlyList<ActivityDefinition> Definitions => _orderedDefinitions;

        public bool TryGet(StableId id, out ActivityDefinition definition)
        {
            return _definitions.TryGetValue(id, out definition!);
        }

        public ActivityDefinition Get(StableId id)
        {
            if (!TryGet(id, out ActivityDefinition definition))
            {
                throw new KeyNotFoundException($"Activity ID '{id}' was not found.");
            }

            return definition;
        }
    }
}
