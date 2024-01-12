using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Scenes
{
    public class ScenesController : Singleton<ScenesController>
    {
        [SerializeField] private ScenesControllerSettings _settings;
        [SerializeField] private LoadingScreen _loadingScreen; 
        
        public UniRx.MessageBroker MessageBroker { get; } = new();

        private string _currentScene;
        private SceneTypes _loadingScene;
        private bool _isSceneLoaded;
        private IDisposable _delayListener;

        protected override void InitializeSingleton()
        {
            base.InitializeSingleton();

            MessageBroker
                .Receive<SceneLoadMessage>()
                .Subscribe(OnSceneLoadMessage);
        }
        
        private void OnSceneLoadMessage(SceneLoadMessage msg)
        {
            _currentScene = SceneManager.GetActiveScene().name;

            _loadingScene = msg.Type;
            _isSceneLoaded = false;
            
            _loadingScreen.onFadeComplete += LoadNewScene;
            _loadingScreen.StartLoading(_settings.FadeTimeStart);
        }

        private void LoadNewScene()
        {
            _loadingScreen.onFadeComplete -= LoadNewScene;

            AsyncOperation newScene = SceneManager
                .LoadSceneAsync(_settings.GetSceneName(_loadingScene), LoadSceneMode.Additive);
            
            newScene.AsObservable().Last()
                .Subscribe(_ => _isSceneLoaded = true)
                .AddTo(this);
            
            _loadingScreen.ShowProgress();
           
            float progress = 0;
            _delayListener = Observable
                .Interval(TimeSpan.FromSeconds(_settings.LoadingDelay / _settings.CountFilling))
                .Subscribe(_ =>
                {
                    progress += Mathf.Clamp01(1 / _settings.CountFilling);
                    _loadingScreen.SetProgress(progress);

                    if (progress < 1 || _isSceneLoaded is false) return;
                    
                    MessageBroker.Publish(new SceneReadyMessage {Type =_loadingScene});
                    
                    UnloadCurrentScene();
                    _delayListener.Dispose();
                });
        }

        private void UnloadCurrentScene()
        {
            SceneManager
                .UnloadSceneAsync(_currentScene)
                .AsObservable()
                .Last()
                .Subscribe(_ => _loadingScreen.EndLoading(_settings.FadeTimeEnd))
                .AddTo(this);
        }
    }
}