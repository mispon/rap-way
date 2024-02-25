using System;
using Game;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Social.Tabs
{
    /// <summary>
    /// Базовый класс вкладки социального действия
    /// </summary>
    public abstract class BaseSocialsTab : MonoBehaviour
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite cooldownSprite;
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
            if (CheckStartConditions())
            {
                startButton.interactable = true;
                startButton.image.sprite = normalSprite;
            } else
            {
                startButton.interactable = false;
                startButton.image.sprite = cooldownSprite;
            }
        }

        /// <summary>
        /// Проверяет условия запуска соц. действия
        /// </summary>
        protected virtual bool CheckStartConditions()
        {
            return GameManager.Instance.GameStats.SocialsCooldown == 0;
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