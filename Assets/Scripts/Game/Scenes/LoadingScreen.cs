using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scenes
{
    [RequireComponent(typeof(Canvas))]
    public class LoadingScreen : MonoBehaviour, IDisposable
    {
        [SerializeField] private Image imageFadeOut;
        [SerializeField] private Image loadingLabel;
        [SerializeField] private RectTransform progressBarParent;
        [SerializeField] private Image progressBar;
        [SerializeField] private bool showProgressBar;

        private Canvas _canvas;
        private bool _isFadeIn;
        
        private IDisposable _fadeDisposable;
        private IDisposable _progressDisposable;
        
        public Action onFadeComplete;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;

            HideProgressBar();
            
            Color color = imageFadeOut.color;
            color.a = 0;
            imageFadeOut.color = color;
        }
        
        public void StartLoading(float fadeTime)
        {
            _isFadeIn = true;
            _canvas.enabled = true;
            
            HideProgressBar();
            StartFade(fadeTime);
        }
        
        public void EndLoading(float fadeTime)
        {
            _isFadeIn = false;
            onFadeComplete += Dispose;
            
            HideProgressBar();
            StartFade(fadeTime);
        }

        private void StartFade(float fadeTime)
        {
            float opacity = 0;
            _fadeDisposable = Observable
                .EveryUpdate()
                .TakeWhile(_ => opacity < 1)
                .Finally(() =>
                {
                    _fadeDisposable?.Dispose();
                    onFadeComplete?.Invoke();
                })
                .Subscribe(_ =>
                {
                    opacity += UnityEngine.Time.deltaTime / fadeTime;
                    float alpha = _isFadeIn ? opacity : 1 - opacity;
                    
                    Color color = imageFadeOut.color;
                    color.a = alpha;
                    imageFadeOut.color = color;
                });
        }
        
        public IObservable<bool> ShowProgressBar(float duration)
        {
            if (!showProgressBar) 
                return Observable.Return(true);
            
            SetLabelsActive(true);

            return Observable.Create<bool>(observer =>
            {
                return Observable
                    .EveryUpdate()
                    .TakeWhile(_ => progressBar.fillAmount < 1)
                    .Finally(observer.OnCompleted)
                    .Subscribe(_ =>
                    {
                        progressBar.fillAmount += UnityEngine.Time.deltaTime / duration;
                    });
            });
        }

        private void HideProgressBar()
        {
            progressBar.fillAmount = 0;
            SetLabelsActive(false);
        }
        
        public void Dispose()
        {
            _fadeDisposable?.Dispose();
            _progressDisposable?.Dispose();
            
            _canvas.enabled = false;
            onFadeComplete = null;

            SetLabelsActive(false);
        }

        private void SetLabelsActive(bool activity)
        {
            progressBarParent.gameObject.SetActive(activity);
            loadingLabel.gameObject.SetActive(activity);
        }
    }
}