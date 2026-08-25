using System;
using RapWay.Application.Activities;
using RapWay.Infrastructure.Activities;
using RapWay.Infrastructure.Content;
using UnityEngine;
using VContainer;

namespace RapWay.Composition.Unity.Content
{
    public static class GameContentInstaller
    {
        private const string ActivityDefinitionsResourcePath = "Content/activities";

        public static void InstallGameContent(this IContainerBuilder builder)
        {
            TextAsset activityDefinitions = Resources.Load<TextAsset>(ActivityDefinitionsResourcePath);
            if (activityDefinitions == null)
            {
                throw new InvalidOperationException(
                    $"Gameplay content resource '{ActivityDefinitionsResourcePath}' was not found.");
            }

            GameContentCatalog catalog = new GameContentCatalogLoader(
                new ActivityDefinitionJsonDeserializer()).Load(activityDefinitions.text);
            builder.RegisterInstance(catalog);
            builder.RegisterInstance<IActivityDefinitionLookup>(catalog);
            builder.RegisterInstance<IActivityDefinitionCatalog>(catalog);
        }
    }
}
