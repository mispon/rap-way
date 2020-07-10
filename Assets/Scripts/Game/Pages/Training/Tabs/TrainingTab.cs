using System;
using UnityEngine;

namespace Game.Pages.Training.Tabs
{
    /// <summary>
    /// Базовый класс вкладок страницы тренировок
    /// </summary>
    public abstract class TrainingTab : MonoBehaviour
    {
        /// <summary>
        /// Активирует / деактивирует вкладку
        /// </summary>
        public abstract void Toggle(bool isOpen);

        /// <summary>
        /// Запускает выполнение тренировки
        /// </summary>
        public Action<int, Action> onStartTraining = (i, action) => {};
    }
}