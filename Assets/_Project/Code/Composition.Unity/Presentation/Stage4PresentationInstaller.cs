using RapWay.Application.Navigation;
using RapWay.Application.Persistence;
using RapWay.Application.Session;
using RapWay.Composition.Unity.Navigation;
using RapWay.Infrastructure.Persistence;
using RapWay.Presentation.Unity.Shell;
using VContainer;

namespace RapWay.Composition.Unity.Presentation
{
    public static class Stage4PresentationInstaller
    {
        public static void InstallStage4Presentation(this IContainerBuilder builder)
        {
            builder.Register<ISceneNavigator, UnitySceneNavigator>(Lifetime.Singleton);
            builder.Register<IGameSessionLaunchRequest, GameSessionLaunchRequest>(Lifetime.Singleton);
            builder.Register<ICareerSaveAvailabilityProbe, CareerSaveAvailabilityProbe>(Lifetime.Singleton);
            builder.Register<IAppShellFlow, AppShellController>(Lifetime.Singleton);
        }
    }
}
