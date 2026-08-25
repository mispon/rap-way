#nullable enable

using System;
using System.Collections.Generic;

namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class UiNavigator : IUiNavigator
    {
        private readonly UiRouteRegistry _registry;
        private readonly int _historyLimit;
        private readonly List<UiRouteEntry> _history = new();
        private UiRouteEntry? _activeDialog;

        public UiNavigator(UiRouteRegistry registry, int historyLimit)
        {
            if (historyLimit < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(historyLimit), historyLimit, "History limit must be at least one.");
            }

            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _historyLimit = historyLimit;
            Snapshot = CreateSnapshot();
        }

        public event Action<UiNavigationSnapshot>? Changed;

        public UiNavigationSnapshot Snapshot { get; private set; }

        public void InitializeHome()
        {
            GoHome();
        }

        public void Navigate(UiRouteId routeId)
        {
            Navigate(routeId, EmptyUiRouteContext.Instance);
        }

        public void Navigate(UiRouteId routeId, IUiRouteContext context)
        {
            if (_activeDialog != null)
            {
                throw new InvalidOperationException("Dismiss the active dialog before navigating to another route.");
            }

            UiRouteDefinition definition = _registry.Get(routeId);
            definition.EnsureAccepts(context);

            switch (definition.NavigationMode)
            {
                case UiNavigationMode.Home:
                    GoHome();
                    return;
                case UiNavigationMode.Push:
                    EnsureHomeInitialized();
                    Push(new UiRouteEntry(definition, context));
                    break;
                case UiNavigationMode.Replace:
                    EnsureHomeInitialized();
                    Replace(new UiRouteEntry(definition, context));
                    break;
                case UiNavigationMode.Dialog:
                    EnsureHomeInitialized();
                    _activeDialog = new UiRouteEntry(definition, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(definition.NavigationMode), definition.NavigationMode, "Unknown navigation mode.");
            }

            Publish();
        }

        public bool TryGoBack()
        {
            if (DismissDialog())
            {
                return true;
            }

            if (_history.Count <= 1)
            {
                return false;
            }

            _history.RemoveAt(_history.Count - 1);
            Publish();
            return true;
        }

        public bool DismissDialog()
        {
            if (_activeDialog == null)
            {
                return false;
            }

            _activeDialog = null;
            Publish();
            return true;
        }

        public void GoHome()
        {
            UiRouteDefinition homeDefinition = _registry.Get(UiRouteId.Home);
            EmptyUiRouteContext context = EmptyUiRouteContext.Instance;
            homeDefinition.EnsureAccepts(context);

            _activeDialog = null;
            _history.Clear();
            _history.Add(new UiRouteEntry(homeDefinition, context));
            Publish();
        }

        public void UpdateCurrentLocalState(IUiRouteLocalState localState)
        {
            if (localState == null)
            {
                throw new ArgumentNullException(nameof(localState));
            }

            if (_activeDialog != null)
            {
                throw new InvalidOperationException("Dismiss the active dialog before updating the origin route state.");
            }

            EnsureHomeInitialized();
            int currentIndex = _history.Count - 1;
            _history[currentIndex] = _history[currentIndex].WithLocalState(localState);
            Publish();
        }

        private void Push(UiRouteEntry entry)
        {
            while (_history.Count >= _historyLimit && _history.Count > 1)
            {
                _history.RemoveAt(1);
            }

            if (_history.Count >= _historyLimit)
            {
                throw new InvalidOperationException("Navigation history limit does not leave room for routes above Home.");
            }

            _history.Add(entry);
        }

        private void Replace(UiRouteEntry entry)
        {
            if (_history.Count == 1)
            {
                Push(entry);
                return;
            }

            _history[_history.Count - 1] = entry;
        }

        private void EnsureHomeInitialized()
        {
            if (_history.Count == 0)
            {
                throw new InvalidOperationException("Initialize Home before navigating to another route.");
            }
        }

        private UiNavigationSnapshot CreateSnapshot()
        {
            return new UiNavigationSnapshot(_history, _activeDialog);
        }

        private void Publish()
        {
            Snapshot = CreateSnapshot();
            Changed?.Invoke(Snapshot);
        }
    }
}
