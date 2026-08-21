using System;
using NUnit.Framework;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;
using RapWay.Domain.Localization;
using RapWay.Domain.Numerics;
using RapWay.Infrastructure.Activities;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class ActivityDefinitionTests
    {
        [Test]
        public void DefinitionAcceptsOnlyItsConfiguredDurationRange()
        {
            ActivityDefinition definition = CreateDefinition(4, 8);

            Assert.That(definition.SupportsDuration(3), Is.False);
            Assert.That(definition.SupportsDuration(4), Is.True);
            Assert.That(definition.SupportsDuration(8), Is.True);
            Assert.That(definition.SupportsDuration(9), Is.False);
        }

        [Test]
        public void DefinitionRejectsDurationsOutsideTheSessionLimit()
        {
            Assert.That(
                (Action)(() => CreateDefinition(1, ActivitySessionState.MaximumDurationHours + 1)),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void DefinitionRejectsAnUnknownCategory()
        {
            Assert.That(
                (Action)(() => new ActivityDefinition(
                    StableId.Create("work.courier"),
                    (ActivityCategory)999,
                    4,
                    8,
                    new LocalizationKey("Activities", "work.courier.title"),
                    new LocalizationKey("Activities", "work.courier.description"),
                    new ActivityHourlyEffects(0, 0, 0, Array.Empty<ActivitySkillExperienceGrant>()),
                    Money.Zero,
                    default,
                    canInterrupt: true,
                    canAccelerateWhenFamiliar: false)),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void JsonDeserializerCreatesAValidatedCatalog()
        {
            const string json = @"{
  'activities': [
    {
      'id': 'work.courier',
      'category': 'work',
      'minimumDurationHours': 4,
      'maximumDurationHours': 8,
      'titleLocalization': { 'table': 'Activities', 'key': 'work.courier.title' },
      'descriptionLocalization': { 'table': 'Activities', 'key': 'work.courier.description' },
      'hourlyEffects': {
        'energyChange': -45,
        'satietyChange': -20,
        'motivationChange': 0,
        'skillExperience': [{ 'skillId': 'skill.endurance', 'experiencePerHour': 3 }]
      },
      'paymentPerCompletedHourMinorUnits': '2500',
      'eventPoolId': 'events.work.courier',
      'canInterrupt': true,
      'canAccelerateWhenFamiliar': true
    }
  ]
}";
            ActivityDefinitionCatalog catalog = new ActivityDefinitionJsonDeserializer().Deserialize(json);

            ActivityDefinition definition = catalog.Get(StableId.Create("work.courier"));

            Assert.That(definition.Category, Is.EqualTo(ActivityCategory.Work));
            Assert.That(definition.SupportsDuration(6), Is.True);
            Assert.That(definition.TitleLocalizationKey, Is.EqualTo(new LocalizationKey("Activities", "work.courier.title")));
            Assert.That(definition.HourlyEffects.EnergyChange, Is.EqualTo(-45));
            Assert.That(definition.HourlyEffects.SkillExperience[0].ExperiencePerHour, Is.EqualTo(3));
            Assert.That(definition.CalculatePayment(3).MinorUnits, Is.EqualTo(7_500));
            Assert.That(definition.HasEventPool, Is.True);
        }

        [Test]
        public void JsonDeserializerRejectsUnknownFields()
        {
            const string json = @"{
  'activities': [],
  'unknown': true
}";

            Assert.That(
                (Action)(() => new ActivityDefinitionJsonDeserializer().Deserialize(json)),
                Throws.TypeOf<FormatException>());
        }

        [Test]
        public void DefinitionCalculatesPaymentOnlyForCompletedHours()
        {
            ActivityDefinition definition = CreateDefinition(4, 8);

            Assert.That(definition.CalculatePayment(0), Is.EqualTo(Money.Zero));
            Assert.That(definition.CalculatePayment(4).MinorUnits, Is.EqualTo(10_000));
            Assert.That(
                (Action)(() => definition.CalculatePayment(9)),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        private static ActivityDefinition CreateDefinition(int minimumDurationHours, int maximumDurationHours)
        {
            return new ActivityDefinition(
                StableId.Create("work.courier"),
                ActivityCategory.Work,
                minimumDurationHours,
                maximumDurationHours,
                new LocalizationKey("Activities", "work.courier.title"),
                new LocalizationKey("Activities", "work.courier.description"),
                new ActivityHourlyEffects(
                    energyChange: -45,
                    satietyChange: -20,
                    motivationChange: 0,
                    new[] { new ActivitySkillExperienceGrant(StableId.Create("skill.endurance"), 3) }),
                Money.FromMinorUnits(2_500),
                StableId.Create("events.work.courier"),
                canInterrupt: true,
                canAccelerateWhenFamiliar: true);
        }
    }
}
