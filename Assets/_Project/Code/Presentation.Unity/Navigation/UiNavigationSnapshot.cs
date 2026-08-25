#nullable enable

using System;
using System.Collections.Generic;

namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class UiNavigationSnapshot
    {
        private readonly UiRouteEntry[] _history;

        public UiNavigationSnapshot(IReadOnlyList<UiRouteEntry> history, UiRouteEntry? activeDialog)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            _history = new UiRouteEntry[history.Count];
            for (int index = 0; index < history.Count; index++)
            {
                _history[index] = history[index] ?? throw new ArgumentException(
                    "Navigation history cannot contain null entries.",
                    nameof(history));
            }

            ActiveDialog = activeDialog;
        }

        public IReadOnlyList<UiRouteEntry> History => _history;

        public UiRouteEntry? ActiveDialog { get; }

        public UiRouteEntry? CurrentRoute => _history.Length == 0 ? null : _history[_history.Length - 1];

        public bool CanGoBack => ActiveDialog != null || _history.Length > 1;
    }
}
