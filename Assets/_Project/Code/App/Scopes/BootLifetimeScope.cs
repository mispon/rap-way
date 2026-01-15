using RapWay.App.Boot;
using VContainer;
using VContainer.Unity;

namespace RapWay.App.Scopes
{
    public class BootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BootController>();
        }
    }
}