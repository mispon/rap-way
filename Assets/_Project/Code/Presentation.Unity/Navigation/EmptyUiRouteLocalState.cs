namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class EmptyUiRouteLocalState : IUiRouteLocalState
    {
        public static EmptyUiRouteLocalState Instance { get; } = new EmptyUiRouteLocalState();

        private EmptyUiRouteLocalState()
        {
        }
    }
}
