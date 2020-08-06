using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Game.Notifications
{
    /// <summary>
    /// Менеджер уведомлений, отображаемых в любой момент игры
    /// </summary>
    public class IndependentNotificationManager: Singleton<IndependentNotificationManager>
    {
        /// <summary>
        /// Текущее состояние очереди: показывается сейчас что-нибудь или нет
        /// </summary>
        private bool _isShown;
        
        /// <summary>
        /// Очередь независимых событий, отображаемых в любой момент игры
        /// </summary>
        private readonly Queue<Action> _notificationActions = new Queue<Action>();
        
        /// <summary>
        /// Добавляет новое уведомление в очередь; отображет его, если показ разрешен
        /// </summary>
        public void AddNotification(Action action)
        {
            _notificationActions.Enqueue(action);
            if (!_isShown)
                CheckNotificationsStatus();
        }

        /// <summary>
        /// Разрешает показ новых уведомлений; показывает, если есть.
        /// Вызов функции полностью лежит на источнике вызова.
        /// </summary>
        public void Unlock()
        {
            _isShown = false;
            CheckNotificationsStatus();
        }

        /// <summary>
        /// Отображает новое уведомление
        /// </summary>
        private void ProcessNotification()
        {
            var action = _notificationActions.Dequeue();
            action.Invoke();
        }

        /// <summary>
        /// Проверяет наличие уведомлений
        /// </summary>
        private void CheckNotificationsStatus()
        {
            _isShown = _notificationActions.Any();
            if(_isShown)
                ProcessNotification();
        }
    }
}