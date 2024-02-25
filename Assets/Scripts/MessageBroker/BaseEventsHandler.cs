using Core.OrderedStarter;
using UniRx;
using UnityEngine;

namespace MessageBroker
{
    public abstract class BaseEventsHandler : MonoBehaviour, IStarter
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