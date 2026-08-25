namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class EmptyUiRouteContext : IUiRouteContext
    {
        public static EmptyUiRouteContext Instance { get; } = new EmptyUiRouteContext();

        private EmptyUiRouteContext()
        {
        }
    }
}
