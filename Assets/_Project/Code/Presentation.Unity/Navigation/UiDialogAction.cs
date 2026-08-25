using System;
using RapWay.Domain.Localization;

namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class UiDialogAction
    {
        public UiDialogAction(string id, LocalizationKey labelKey, bool isPrimary)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("A dialog action ID is required.", nameof(id));
            }

            Id = id;
            LabelKey = labelKey;
            IsPrimary = isPrimary;
        }

        public string Id { get; }

        public LocalizationKey LabelKey { get; }

        public bool IsPrimary { get; }
    }
}
