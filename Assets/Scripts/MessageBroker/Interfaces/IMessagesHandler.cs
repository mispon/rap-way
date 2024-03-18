using UniRx;

namespace MessageBroker.Interfaces
{
    public interface IMessagesHandler
    {
        public void RegisterHandlers(CompositeDisposable disposable);
    }
}