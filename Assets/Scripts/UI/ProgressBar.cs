using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    /// <summary>
    /// Реализация прогресс бара
    /// </summary>
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;

        public bool IsFinish { get; private set; }
        public Action onFinish = () => {};
        
        private int _duration;
        
        private readonly WaitForSeconds _beforeFinishDelay = new WaitForSeconds(2f);
        
        /// <summary>
        /// Инициализация прогресс бара
        /// </summary>
        public void Init(int duration)
        {
            _duration = duration;
            progressBar.value = 0;
            IsFinish = false;
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
                progressBar.value = Mathf.Lerp(0f, 1f, elapsedTime / _duration);
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