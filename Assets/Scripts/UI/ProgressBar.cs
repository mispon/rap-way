using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private float progressChangeSpeed;

        public Action onFinish = () => {};
        
        private int _duration;
        private int _progress;
        
        public void Init(int duration)
        {
            _duration = duration;
        }

        /// <summary>
        /// Увеличивает текущее значение прогресса 
        /// </summary>
        public void AddProgress(int value)
        {
            _progress += value;
            DisplayProgress();
        }

        /// <summary>
        /// Сбрасывает состояние прогресс бара
        /// </summary>
        public void ResetProgress()
        {
            _progress = 0;
            progressBar.value = 0;
        }

        /// <summary>
        /// Запускает корутину изменения прогресс бара
        /// </summary>
        private void DisplayProgress()
        {
            StartCoroutine(ProgressDisplayCoroutine());
        }

        /// <summary>
        /// Корутина изменения прогресс бара
        /// </summary>
        private IEnumerator ProgressDisplayCoroutine()
        {
            while (progressBar.value < _duration / 100f * _progress)
            {
                progressBar.value += Time.deltaTime * progressChangeSpeed;
                yield return null;
            }
            
            if (progressBar.value > 0.99f)
                onFinish.Invoke();
        }
    }
}