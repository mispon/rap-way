using UnityEngine;

namespace Core
{
    /// <summary>
    /// Базовый класс для подписки на события течения времени
    /// </summary>
    public class TimeListener : MonoBehaviour
    {
        private void Start()
        {
            TimeManager.Instance.onDayLeft += OnDayLeft;
            TimeManager.Instance.onWeekLeft += OnWeekLeft;
            TimeManager.Instance.onMonthLeft += OnMonthLeft;
        }

        /// <summary>
        /// Обработчик завершения дня
        /// </summary>
        protected virtual void OnDayLeft() {}
        
        /// <summary>
        /// Обработчик завершения недели
        /// </summary>
        protected virtual void OnWeekLeft() {}
        
        /// <summary>
        /// Обработчик завершения месяца
        /// </summary>
        protected virtual void OnMonthLeft() {}

        private void OnDisable()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
            TimeManager.Instance.onWeekLeft -= OnWeekLeft;
            TimeManager.Instance.onMonthLeft -= OnMonthLeft;
        }
    }
}