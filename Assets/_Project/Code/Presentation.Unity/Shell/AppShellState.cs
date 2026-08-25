using System;
using RapWay.Application.Session;
using RapWay.Presentation.Unity.Navigation;

namespace RapWay.Presentation.Unity.Shell
{
    public readonly struct AppShellState : IEquatable<AppShellState>
    {
        public AppShellState(
            AppShellScreen activeScreen,
            DialogUiRouteContext activeDialog,
            bool canContinue,
            CareerStartTemplateId? selectedStartTemplateId,
            bool isBusy,
            string statusText)
        {
            ActiveScreen = activeScreen;
            ActiveDialog = activeDialog;
            CanContinue = canContinue;
            SelectedStartTemplateId = selectedStartTemplateId;
            IsBusy = isBusy;
            StatusText = statusText ?? string.Empty;
        }

        public AppShellScreen ActiveScreen { get; }

        public DialogUiRouteContext ActiveDialog { get; }

        public bool CanContinue { get; }

        public CareerStartTemplateId? SelectedStartTemplateId { get; }

        public bool IsBusy { get; }

        public string StatusText { get; }

        public AppShellState With(
            AppShellScreen? activeScreen = null,
            DialogUiRouteContext activeDialog = null,
            bool clearActiveDialog = false,
            bool? canContinue = null,
            CareerStartTemplateId? selectedStartTemplateId = null,
            bool clearSelectedStartTemplateId = false,
            bool? isBusy = null,
            string statusText = null)
        {
            CareerStartTemplateId? resolvedTemplateId = clearSelectedStartTemplateId
                ? null
                : (selectedStartTemplateId ?? SelectedStartTemplateId);

            return new AppShellState(
                activeScreen ?? ActiveScreen,
                clearActiveDialog ? null : (activeDialog ?? ActiveDialog),
                canContinue ?? CanContinue,
                resolvedTemplateId,
                isBusy ?? IsBusy,
                statusText ?? StatusText);
        }

        public bool Equals(AppShellState other)
        {
            return ActiveScreen == other.ActiveScreen &&
                   Equals(ActiveDialog, other.ActiveDialog) &&
                   CanContinue == other.CanContinue &&
                   SelectedStartTemplateId == other.SelectedStartTemplateId &&
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
                hashCode = (hashCode * 397) ^ (ActiveDialog != null ? ActiveDialog.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CanContinue.GetHashCode();
                hashCode = (hashCode * 397) ^ (SelectedStartTemplateId.HasValue ? (int)SelectedStartTemplateId.Value : -1);
                hashCode = (hashCode * 397) ^ IsBusy.GetHashCode();
                hashCode = (hashCode * 397) ^ (StatusText != null ? StatusText.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
