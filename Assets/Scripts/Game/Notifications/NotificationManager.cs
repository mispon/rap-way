using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Notifications
{
    /// <summary>
    /// Менеджер уведомлений
    /// </summary>
    public class NotificationManager : Singleton<NotificationManager>
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
            SoundManager.Instance.PlayNotify();
            _notificationActions.Enqueue(action);
            CheckStatus();
        }

        /// <summary>
        /// Обрабатывает нажатие на иконку уведомлений
        /// </summary>
        private void ProcessNotification()
        {
            SoundManager.Instance.PlayClick();
            var action = _notificationActions.Dequeue();
            action.Invoke();
            CheckStatus();
        }

        /// <summary>
        /// Проверяет наличие уведомлений
        /// </summary>
        private void CheckStatus()
        {
            bool hasNotifications = _notificationActions.Any();
            notificationButton.gameObject.SetActive(hasNotifications);
        }
    }
}