using System;
using RapWay.Domain.Common;
using RapWay.Domain.Localization;
using RapWay.Domain.Numerics;

namespace RapWay.Domain.Activities
{
    public sealed class ActivityDefinition
    {
        public ActivityDefinition(
            StableId id,
            ActivityCategory category,
            int minimumDurationHours,
            int maximumDurationHours,
            LocalizationKey titleLocalizationKey,
            LocalizationKey descriptionLocalizationKey,
            ActivityHourlyEffects hourlyEffects,
            Money paymentPerCompletedHour,
            StableId eventPoolId,
            bool canInterrupt,
            bool canAccelerateWhenFamiliar)
        {
            if (!id.IsValid)
            {
                throw new ArgumentException("Activity ID must be valid.", nameof(id));
            }

            if (!Enum.IsDefined(typeof(ActivityCategory), category))
            {
                throw new ArgumentOutOfRangeException(nameof(category));
            }

            if (minimumDurationHours <= 0 || maximumDurationHours < minimumDurationHours ||
                maximumDurationHours > ActivitySessionState.MaximumDurationHours)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumDurationHours));
            }

            Id = id;
            Category = category;
            MinimumDurationHours = minimumDurationHours;
            MaximumDurationHours = maximumDurationHours;
            TitleLocalizationKey = titleLocalizationKey;
            DescriptionLocalizationKey = descriptionLocalizationKey;
            HourlyEffects = hourlyEffects ?? throw new ArgumentNullException(nameof(hourlyEffects));
            if (paymentPerCompletedHour.MinorUnits < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(paymentPerCompletedHour));
            }

            PaymentPerCompletedHour = paymentPerCompletedHour;
            EventPoolId = eventPoolId;
            CanInterrupt = canInterrupt;
            CanAccelerateWhenFamiliar = canAccelerateWhenFamiliar;
        }

        public StableId Id { get; }

        public ActivityCategory Category { get; }

        public int MinimumDurationHours { get; }

        public int MaximumDurationHours { get; }

        public LocalizationKey TitleLocalizationKey { get; }

        public LocalizationKey DescriptionLocalizationKey { get; }

        public ActivityHourlyEffects HourlyEffects { get; }

        public Money PaymentPerCompletedHour { get; }

        public StableId EventPoolId { get; }

        public bool HasEventPool => EventPoolId.IsValid;

        public bool CanInterrupt { get; }

        public bool CanAccelerateWhenFamiliar { get; }

        public bool SupportsDuration(int durationHours)
        {
            return durationHours >= MinimumDurationHours && durationHours <= MaximumDurationHours;
        }

        public Money CalculatePayment(int completedHours)
        {
            if (completedHours < 0 || completedHours > MaximumDurationHours)
            {
                throw new ArgumentOutOfRangeException(nameof(completedHours));
            }

            return Money.FromMinorUnits(checked(PaymentPerCompletedHour.MinorUnits * completedHours));
        }
    }
}
