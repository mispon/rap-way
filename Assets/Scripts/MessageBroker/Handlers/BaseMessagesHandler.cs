using Core.OrderedStarter;
using UniRx;
using UnityEngine;

namespace MessageBroker.Handlers
{
    public abstract class BaseMessagesHandler : MonoBehaviour, IStarter
    {
        protected readonly CompositeDisposable disposable = new();
        
        public void OnStart()
        {
            RegisterHandlers();
        }
        
        protected abstract void RegisterHandlers();

        private void OnDestroy()
        {
            disposable.Clear();
        }
    }
}