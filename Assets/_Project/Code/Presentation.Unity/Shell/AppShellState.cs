using System;

namespace RapWay.Presentation.Unity.Shell
{
    public readonly struct AppShellState : IEquatable<AppShellState>
    {
        public AppShellState(
            AppShellScreen activeScreen,
            AppShellModal activeModal,
            bool canContinue,
            bool isBusy,
            string statusText)
        {
            ActiveScreen = activeScreen;
            ActiveModal = activeModal;
            CanContinue = canContinue;
            IsBusy = isBusy;
            StatusText = statusText ?? string.Empty;
        }

        public AppShellScreen ActiveScreen { get; }

        public AppShellModal ActiveModal { get; }

        public bool CanContinue { get; }

        public bool IsBusy { get; }

        public string StatusText { get; }

        public AppShellState With(
            AppShellScreen? activeScreen = null,
            AppShellModal? activeModal = null,
            bool? canContinue = null,
            bool? isBusy = null,
            string statusText = null)
        {
            return new AppShellState(
                activeScreen ?? ActiveScreen,
                activeModal ?? ActiveModal,
                canContinue ?? CanContinue,
                isBusy ?? IsBusy,
                statusText ?? StatusText);
        }

        public bool Equals(AppShellState other)
        {
            return ActiveScreen == other.ActiveScreen &&
                   ActiveModal == other.ActiveModal &&
                   CanContinue == other.CanContinue &&
                   IsBusy == other.IsBusy &&
                   string.Equals(StatusText, other.StatusText, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is AppShellState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)ActiveScreen;
                hashCode = (hashCode * 397) ^ (int)ActiveModal;
                hashCode = (hashCode * 397) ^ CanContinue.GetHashCode();
                hashCode = (hashCode * 397) ^ IsBusy.GetHashCode();
                hashCode = (hashCode * 397) ^ (StatusText != null ? StatusText.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
