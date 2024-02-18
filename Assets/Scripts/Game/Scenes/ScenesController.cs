using System.Threading;
using Core;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scenes
{
    public class ScenesController : Singleton<ScenesController>
    {
        [SerializeField] private ScenesControllerSettings settings;
        [SerializeField] private LoadingScreen loadingScreen; 
        
        public UniRx.MessageBroker MessageBroker { get; } = new();

        private string _currentScene;
        private SceneTypes _loadingScene;
        private long _ack;
        
        protected override void InitializeSingleton()
        {
            base.InitializeSingleton();

            MessageBroker
                .Receive<SceneLoadMessage>()
                .Subscribe(OnSceneLoadMessage);
        }
        
        private void OnSceneLoadMessage(SceneLoadMessage msg)
        {
            _loadingScene = msg.SceneType;
            
            loadingScreen.onFadeComplete += LoadNewScene;
            loadingScreen.StartLoading(settings.FadeTimeStart);
        }

        private void LoadNewScene()
        {
            _ack = 0;
            _currentScene = SceneManager.GetActiveScene().name;
            
            loadingScreen.onFadeComplete -= LoadNewScene;
            loadingScreen
                .ShowProgressBar(settings.LoadingDelay)
                .DoOnCompleted(UnloadCurrentScene)
                .Subscribe(e => Debug.Log(e))
                .AddTo(this);
            
            string sceneName = settings.GetSceneName(_loadingScene);
            SceneManager
                .LoadSceneAsync(sceneName, LoadSceneMode.Additive)
                .AsObservable()
                .Last()
                .Subscribe(_ => UnloadCurrentScene());
        }

        private void UnloadCurrentScene()
        {
            // we need 2 acks to unload last scene
            Interlocked.Increment(ref _ack);
            if (Interlocked.Read(ref _ack) != 2)
                return;
            
            SceneManager
                .UnloadSceneAsync(_currentScene)
                .AsObservable()
                .Last()
                .Subscribe(_ => loadingScreen.EndLoading(settings.FadeTimeEnd))
                .AddTo(this);
        }
    }
}