using MessagePipe;
using RapWay.Application.Events;
using RapWay.Infrastructure.Messaging;
using VContainer;

namespace RapWay.Composition.Unity
{
    public static class SimulationMessagingInstaller
    {
        public static void InstallSimulationMessaging(this IContainerBuilder builder)
        {
            builder.RegisterMessagePipe(configure =>
            {
                configure.InstanceLifetime = InstanceLifetime.Scoped;
            });
            builder.Register<MessagePipeCommittedEventSink>(Lifetime.Scoped).As<ICommittedEventSink>();
        }
    }
}
