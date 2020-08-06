using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Notifications
{
    /// <summary>
    /// Менеджер уведомлений, отображаемых по нажатию кнопки
    /// </summary>
    public class ClickNotificationManager : Singleton<ClickNotificationManager>
    {
        [SerializeField] private Button notificationButton;
        
        /// <summary>
        /// Очередь действий по клику на иконку уведомлений
        /// </summary>
        private readonly Queue<Action> _notificationActions = new Queue<Action>();

        private void Start()
        {
            notificationButton.onClick.AddListener(ProcessNotification);
            notificationButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Добавляет новое уведомление в очередь
        /// </summary>
        public void AddNotification(Action action)
        {
            _notificationActions.Enqueue(action);
            CheckNotificationsStatus();
        }

        /// <summary>
        /// Обрабатывает нажатие на иконку уведомлений
        /// </summary>
        private void ProcessNotification()
        {
            var action = _notificationActions.Dequeue();
            action.Invoke();
            CheckNotificationsStatus();
        }

        /// <summary>
        /// Проверяет наличие уведомлений
        /// </summary>
        private void CheckNotificationsStatus()
        {
            bool hasNotifications = _notificationActions.Any();
            notificationButton.gameObject.SetActive(hasNotifications);
        }
    }
}