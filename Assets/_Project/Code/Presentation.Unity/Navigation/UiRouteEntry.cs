#nullable enable

using System;

namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class UiRouteEntry
    {
        public UiRouteEntry(
            UiRouteDefinition definition,
            IUiRouteContext context,
            IUiRouteLocalState? localState = null)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            Definition.EnsureAccepts(context);

            Context = context;
            LocalState = localState ?? EmptyUiRouteLocalState.Instance;
        }

        public UiRouteDefinition Definition { get; }

        public IUiRouteContext Context { get; }

        public IUiRouteLocalState LocalState { get; }

        public UiRouteEntry WithLocalState(IUiRouteLocalState localState)
        {
            if (localState == null)
            {
                throw new ArgumentNullException(nameof(localState));
            }

            return new UiRouteEntry(Definition, Context, localState);
        }
    }
}
