using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
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
        private readonly Queue<Action> _clickNotificationActions = new();

        /// <summary>
        /// Очередь независимых событий, отображаемых в любой момент игры
        /// </summary>
        private readonly Queue<Action> _independentNotificationActions = new();

        /// <summary>
        /// Текущее состояние очереди: показывается сейчас что-нибудь или нет
        /// </summary>
        private bool _isIndependentVisualized;

        private void Start()
        {
            notificationButton.onClick.AddListener(() => ProcessNotification(_clickNotificationActions));
            notificationButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Добавляет новое уведомление в очередь показа по клику
        /// </summary>
        public void AddClickNotification(Action action)
        {
            SoundManager.Instance.PlaySound(UIActionType.Notify);
            
            _clickNotificationActions.Enqueue(action);
            CheckClickNotificationsStatus();
        }

        /// <summary>
        /// Добавляет новое уведомление в очередь независимых; отображет его, если показ разрешен
        /// </summary>
        public void AddIndependentNotification(Action action)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            _independentNotificationActions.Enqueue(action);
            if (!_isIndependentVisualized)
            {
                CheckIndependentNotificationsStatus();
            }
        }
        /// <summary>
        /// Разрешает показ новых уведомлений; показывает, если есть.
        /// Вызов функции полностью лежит на источнике вызова.
        /// </summary>
        public void UnlockIndependentQueue()
        {
            _isIndependentVisualized = false;
            CheckIndependentNotificationsStatus();
        }

        /// <summary>
        /// Проверяет наличие уведомлений по клику
        /// </summary>
        private void CheckClickNotificationsStatus()
        {
            bool hasNotifications = _clickNotificationActions.Any();
            notificationButton.gameObject.SetActive(hasNotifications);
        }

        /// <summary>
        /// Проверяет наличие независимых уведомлений
        /// </summary>
        private void CheckIndependentNotificationsStatus()
        {
            _isIndependentVisualized = _independentNotificationActions.Any();
            if (_isIndependentVisualized)
            {
                ProcessNotification(_independentNotificationActions, false);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие на иконку уведомлений - (isClickNotification)
        /// Обрабатывает следующее в очереди независимое уведомление - (!isClickNotification)
        /// </summary>
        private void ProcessNotification(Queue<Action> actionQueue, bool isClickNotification = true)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            var action = actionQueue.Dequeue();
            action.Invoke();

            if (isClickNotification)
            {
                CheckClickNotificationsStatus();
            }
        }
    }
}