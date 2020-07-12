using System;
using Localization;
using UnityEngine;

namespace Game.Pages.Training.Tabs
{
    /// <summary>
    /// Базовый класс вкладок страницы тренировок
    /// </summary>
    public abstract class TrainingTab : MonoBehaviour
    {
        [Header("Длительность тренировки")]
        [SerializeField] protected int trainingDuration;

        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Активирует / деактивирует вкладку
        /// </summary>
        public void Toggle(bool isOpen)
        {
            if (isOpen)
            {
                OnOpen();
                gameObject.SetActive(true);
            }
            else
                gameObject.SetActive(false);
        }

        /// <summary>
        /// Вызывается в дочерних классах при открытии
        /// </summary>
        protected abstract void OnOpen();

        /// <summary>
        /// Запускает выполнение тренировки
        /// </summary>
        public Action<int, Func<string>> onStartTraining = (duration, callback) => {};

        /// <summary>
        /// Обертка менеджера локализации для краткости кода 
        /// </summary>
        protected static string Locale(string key)
        {
            return LocalizationManager.Instance.Get(key);
        }
    }
}