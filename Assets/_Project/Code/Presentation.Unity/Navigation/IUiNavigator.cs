using System;

namespace RapWay.Presentation.Unity.Navigation
{
    public interface IUiNavigator
    {
        event Action<UiNavigationSnapshot> Changed;

        UiNavigationSnapshot Snapshot { get; }

        void InitializeHome();

        void Navigate(UiRouteId routeId);

        void Navigate(UiRouteId routeId, IUiRouteContext context);

        bool TryGoBack();

        bool DismissDialog();

        void GoHome();

        void UpdateCurrentLocalState(IUiRouteLocalState localState);
    }
}
