using NUnit.Framework;
using RapWay.Domain.Common;
using RapWay.Infrastructure.Activities;
using RapWay.Infrastructure.Content;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class GameContentCatalogLoaderTests
    {
        [Test]
        public void LoaderBuildsAReadOnlyActivityLookupFromValidatedJson()
        {
            const string json = @"{
  'activities': [
    {
      'id': 'work.courier',
      'category': 'work',
      'minimumDurationHours': 2,
      'maximumDurationHours': 8,
      'titleLocalization': { 'table': 'Activities', 'key': 'work_courier_title' },
      'descriptionLocalization': { 'table': 'Activities', 'key': 'work_courier_description' },
      'hourlyEffects': {
        'energyChange': -50,
        'satietyChange': -25,
        'motivationChange': 0,
        'skillExperience': []
      },
      'paymentPerCompletedHourMinorUnits': '2500',
      'eventPoolId': null,
      'canInterrupt': true,
      'canAccelerateWhenFamiliar': true
    }
  ]
}";
            GameContentCatalog catalog = new GameContentCatalogLoader(
                new ActivityDefinitionJsonDeserializer()).Load(json);

            bool found = catalog.TryGet(StableId.Create("work.courier"), out var definition);

            Assert.That(found, Is.True);
            Assert.That(definition.Id, Is.EqualTo(StableId.Create("work.courier")));
            Assert.That(definition.HasEventPool, Is.False);
            Assert.That(catalog.Definitions, Has.Count.EqualTo(1));
            Assert.That(catalog.Definitions[0].Id, Is.EqualTo(StableId.Create("work.courier")));
        }

        [Test]
        public void LoaderRejectsContentWithUnknownFields()
        {
            const string json = @"{
  'activities': [],
  'unexpected': true
}";

            Assert.That(
                (System.Action)(() => new GameContentCatalogLoader(new ActivityDefinitionJsonDeserializer()).Load(json)),
                Throws.TypeOf<System.FormatException>());
        }
    }
}
