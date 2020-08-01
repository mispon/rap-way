using System;
using Core;
using Data;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Training.Tabs.ToneTab
{
    /// <summary>
    /// Карточка стилистики в гриде стилистик
    /// </summary>
    public abstract class TrainingToneCard : MonoBehaviour
    {
        [SerializeField] protected Text toneName;
        [SerializeField] protected Text tonePrice;
        [SerializeField] protected Button button;
        [SerializeField] protected TrainingToneView view;
        
        public Action<Enum, int> onUnlock = (tone, cost) => {};

        protected TonesInfo _info;
        private bool _expEnough;
        private bool _locked;

        private void Start()
        {
            button.onClick.AddListener(ShowInfo);
        }

        /// <summary>
        /// Устанавливает данные карточки
        /// </summary>
        public void Setup(int index, TonesInfo info)
        {
            _info = info;

            string nameKey = GetValue().GetDescription();
            toneName.text = LocalizationManager.Instance.Get(nameKey).ToUpper();

            name = $"ToneCard-{index + 1}";
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Возвращает значение 
        /// </summary>
        protected abstract Enum GetValue();
        
        /// <summary>
        /// Возвращает признак закрытости 
        /// </summary>
        protected abstract bool IsLocked();

        /// <summary>
        /// Обновляет состояние карточки
        /// </summary>
        public void Refresh()
        {
            _expEnough = PlayerManager.Data.Exp >= _info.Price;
            _locked = IsLocked();

            tonePrice.text = _locked ? _info.Price.ToString() : "";
            button.image.sprite = _locked ? _info.Locked : _info.Normal;
        }

        /// <summary>
        /// Показывает информацию о стилистике
        /// </summary>
        private void ShowInfo()
        {
            SoundManager.Instance.Click();
            
            var context = new ToneViewContext
            {
                Tone = GetValue(),
                Cost = _info.Price,
                Icon = _info.Normal,
                ExpEnough = _expEnough,
                IsLocked = _locked,
                onClick = onUnlock
            };
            
            view.Show(context);
        }
    }
}