using System;
using Models.Info;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Social.Tabs
{
    /// <summary>
    /// Базовый класс вкладки социального действия
    /// </summary>
    public abstract class BaseSocialsTab : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        
        /// <summary>
        /// Обработчик запуска социального действия
        /// </summary>
        public event Action<SocialInfo> onStartSocial = info => {};

        private void Start()
        {
            startButton.onClick.AddListener(StartSocial);
            TabStart();
        }

        /// <summary>
        /// Управляет видимостью вкладки 
        /// </summary>
        public void SetVisible(bool state)
        {
            if (state)
            {
                OnOpen();
            }

            gameObject.SetActive(state);
        }

        /// <summary>
        /// Обновляет состояние вкладки
        /// </summary>
        public void Refresh()
        {
            OnOpen();
        }

        /// <summary>
        /// Обработчик запуска соц. действия
        /// </summary>
        private void StartSocial()
        {
            var info = GetInfo();
            onStartSocial.Invoke(info);
        }

        /// <summary>
        /// Вызывается при открытии вкладки
        /// </summary>
        protected virtual void OnOpen()
        {
            startButton.interactable = GameManager.Instance.GameStats.SocialsCooldown == 0;
        }

        /// <summary>
        /// Переопределяется вкладками
        /// </summary>
        protected virtual void TabStart() {}
        
        /// <summary>
        /// Возвращает информацию соц. действия 
        /// </summary>
        protected abstract SocialInfo GetInfo();
    }
}