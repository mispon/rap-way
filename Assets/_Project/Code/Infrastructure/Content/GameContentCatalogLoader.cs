using System;
using RapWay.Infrastructure.Activities;

namespace RapWay.Infrastructure.Content
{
    public sealed class GameContentCatalogLoader
    {
        private readonly ActivityDefinitionJsonDeserializer _activityDeserializer;

        public GameContentCatalogLoader(ActivityDefinitionJsonDeserializer activityDeserializer)
        {
            _activityDeserializer = activityDeserializer ?? throw new ArgumentNullException(nameof(activityDeserializer));
        }

        public GameContentCatalog Load(string activityDefinitionsJson)
        {
            ActivityDefinitionCatalog activities = _activityDeserializer.Deserialize(activityDefinitionsJson);
            return new GameContentCatalog(activities);
        }
    }
}
