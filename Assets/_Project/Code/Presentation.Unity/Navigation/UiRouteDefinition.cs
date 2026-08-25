using System;

namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class UiRouteDefinition
    {
        public UiRouteDefinition(
            UiRouteId id,
            UiNavigationMode navigationMode,
            Type contextType,
            UiBackAffordance backAffordance)
        {
            if (contextType == null)
            {
                throw new ArgumentNullException(nameof(contextType));
            }

            if (!typeof(IUiRouteContext).IsAssignableFrom(contextType))
            {
                throw new ArgumentException(
                    $"Route context type '{contextType.FullName}' must implement {nameof(IUiRouteContext)}.",
                    nameof(contextType));
            }

            Id = id;
            NavigationMode = navigationMode;
            ContextType = contextType;
            BackAffordance = backAffordance;
        }

        public UiRouteId Id { get; }

        public UiNavigationMode NavigationMode { get; }

        public Type ContextType { get; }

        public UiBackAffordance BackAffordance { get; }

        public void EnsureAccepts(IUiRouteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!ContextType.IsInstanceOfType(context))
            {
                throw new ArgumentException(
                    $"Route '{Id}' requires context type '{ContextType.Name}', but received '{context.GetType().Name}'.",
                    nameof(context));
            }
        }
    }
}
