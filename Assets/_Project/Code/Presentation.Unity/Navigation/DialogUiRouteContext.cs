using System;
using System.Collections.Generic;
using RapWay.Domain.Localization;

namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class DialogUiRouteContext : IUiRouteContext
    {
        private readonly UiDialogAction[] _actions;

        public DialogUiRouteContext(
            LocalizationKey titleKey,
            LocalizationKey bodyKey,
            IReadOnlyList<UiDialogAction> actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            if (actions.Count == 0)
            {
                throw new ArgumentException("At least one dialog action is required.", nameof(actions));
            }

            _actions = new UiDialogAction[actions.Count];
            bool hasPrimaryAction = false;

            for (int index = 0; index < actions.Count; index++)
            {
                UiDialogAction action = actions[index] ?? throw new ArgumentException(
                    "Dialog actions cannot contain null entries.",
                    nameof(actions));
                _actions[index] = action;
                hasPrimaryAction |= action.IsPrimary;
            }

            if (!hasPrimaryAction)
            {
                throw new ArgumentException("A dialog requires one primary action.", nameof(actions));
            }

            TitleKey = titleKey;
            BodyKey = bodyKey;
        }

        public LocalizationKey TitleKey { get; }

        public LocalizationKey BodyKey { get; }

        public IReadOnlyList<UiDialogAction> Actions => _actions;
    }
}
