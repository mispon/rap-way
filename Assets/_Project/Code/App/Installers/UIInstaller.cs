using RapWay.Core.UI;
using RapWay.Data;
using VContainer;

namespace RapWay.App.Installers
{
    public static class UIInstaller
    {
        public static void InstallUISystems(this IContainerBuilder builder, UIConfig config)
        {
            builder.RegisterInstance(config);
            builder.Register<UIService>(Lifetime.Singleton);
        }
    }
}