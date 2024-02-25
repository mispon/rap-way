using Core;
using UniRx;
using UnityEngine;

namespace Game
{
    public abstract class GamePackage<T>: Singleton<T> where T : MonoBehaviour
    {
        protected readonly CompositeDisposable disposable = new();

        protected abstract void RegisterHandlers();
        
        private void OnDestroy()
        {
            disposable.Clear();
        }
    }
}