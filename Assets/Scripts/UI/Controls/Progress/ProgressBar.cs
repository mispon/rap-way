using System;
using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controls.Progress
{
    /// <summary>
    /// Реализация прогресс бара
    /// </summary>
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image progress;

        public bool IsFinish { get; private set; }
        public Action onFinish = () => {};
        
        private int _duration;
        
        private readonly WaitForSeconds _beforeFinishDelay = new WaitForSeconds(2f);
        
        /// <summary>
        /// Инициализация прогресс бара
        /// </summary>
        public void Init(int duration)
        {
            if (duration == 0)
                throw new RapWayException("Передано нулевое значение длительности в ProgressBar!");
            
            _duration = duration;
            progress.fillAmount = 0;
            IsFinish = false;
        }

        /// <summary>
        /// Устанавливает текущее значение 
        /// </summary>
        public void SetValue(int value, int maxValue)
        {
            float amount = 1f * value / maxValue;
            progress.fillAmount = amount;
        }

        /// <summary>
        /// Запускает прогресс бар
        /// </summary>
        public void Run()
        {
            StartCoroutine(ProgressDisplayCoroutine());
        }

        /// <summary>
        /// Корутина изменения прогресс бара
        /// </summary>
        private IEnumerator ProgressDisplayCoroutine()
        {
            var elapsedTime = 0f;
            while (elapsedTime < _duration)
            {
                progress.fillAmount = Mathf.Lerp(0f, 1f, elapsedTime / _duration);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            IsFinish = true;
            yield return DelayAndFinishCoroutine();
        }

        /// <summary>
        /// Пауза перед завершением работы 
        /// </summary>
        private IEnumerator DelayAndFinishCoroutine()
        {
            yield return _beforeFinishDelay;
            onFinish.Invoke();
        }
    }
}