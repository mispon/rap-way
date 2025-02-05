using System;
using System.Threading;
using Core;
using MessageBroker;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using UI.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes
{
    public class ScenesController : Singleton<ScenesController>
    {
        [SerializeField] private ScenesControllerSettings settings;
        [SerializeField] private LoadingScreen loadingScreen; 
        
        private string _currentScene;
        private SceneType _loadingScene;
        private long _ack;
        
        protected override void Initialize()
        {
            SceneMessageBroker.Instance
                .Receive<SceneLoadMessage>()
                .Subscribe(OnSceneLoadMessage)
                .AddTo(this);;
        }
        
        private void OnSceneLoadMessage(SceneLoadMessage msg)
        {
            _loadingScene = msg.SceneType;
            
            loadingScreen.onFadeComplete += LoadNewScene;
            loadingScreen.StartLoading(settings.FadeTimeStart);
        }

        private void LoadNewScene()
        {
            try
            {
                _ack          = 0;
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
                    .LastOrDefault()
                    .Subscribe(_ => UnloadCurrentScene())
                    .AddTo(this);

            } catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private void UnloadCurrentScene()
        {
            try
            {
                // we need 2 ACKs to unload last scene
                Interlocked.Increment(ref _ack);
                if (Interlocked.Read(ref _ack) != 2)
                    return;

                SceneManager
                    .UnloadSceneAsync(_currentScene)
                    .AsObservable()
                    .LastOrDefault()
                    .Subscribe(_ => loadingScreen.EndLoading(settings.FadeTimeEnd))
                    .AddTo(this);

                MsgBroker.Instance.Publish(new SceneLoadedMessage());
                
            } catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}