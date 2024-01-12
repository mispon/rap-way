using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes
{
    [RequireComponent(typeof(Canvas))]
    public class LoadingScreen : MonoBehaviour, IDisposable
    {
        [SerializeField] private Image _imageFadeOut;
        [SerializeField] private RectTransform _progressBarParent;
        [SerializeField] private Image _progressBar;
        [SerializeField] private bool _showProgressBar;

        private Canvas _canvas;
        private IDisposable _fadeDisposable;
        private bool _isFadeIn;
        
        public Action onFadeComplete;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;

            HideProgress();
            Color color = _imageFadeOut.color;
            color.a = 0;
            _imageFadeOut.color = color;
        }
        
        public void StartLoading(float fadeTime)
        {
            _canvas.enabled = true;
            HideProgress();
            _isFadeIn = true;
            StartFade(fadeTime);
        }
        
        public void EndLoading(float fadeTime)
        {
            HideProgress();
            _isFadeIn = false;
            onFadeComplete += Dispose;
            StartFade(fadeTime);
        }

        private void StartFade(float fadeTime)
        {
            _fadeDisposable?.Dispose();

            float opacity = 0;
            _fadeDisposable = Observable
                .EveryUpdate()
                .TakeWhile(_ => opacity <= 1)
                .Finally(() => onFadeComplete?.Invoke())
                .Subscribe(_ =>
                {
                    opacity += Time.deltaTime / fadeTime;
                   
                    float alpha = _isFadeIn ? opacity : 1 - opacity;
                    Color color = _imageFadeOut.color;
                    color.a = alpha;
                    _imageFadeOut.color = color;
                });
        }
        
        public void ShowProgress()
        {
            if (!_showProgressBar) 
                return;
            
            _progressBarParent.gameObject.SetActive(true);
        }

        private void HideProgress()
        {
            SetProgress(0);
            _progressBarParent.gameObject.SetActive(false);
        }

        public void SetProgress(float loadProgressRatio)
        {
            if (!_showProgressBar) 
                return;
            
            _progressBar.fillAmount = loadProgressRatio;
        }
        
        public void Dispose()
        {
            onFadeComplete = null;
            _fadeDisposable?.Dispose();
            _progressBarParent.gameObject.SetActive(false);
            _canvas.enabled = false;
        }
    }
}