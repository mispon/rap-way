using System;
using Enums;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Training.Tabs.ToneTab
{
    /// <summary>
    /// Карточка стилистики в гриде стилистик
    /// </summary>
    public class TrainingToneCard<T> : MonoBehaviour 
        where T : Enum
    {
        [SerializeField] private Text toneName;
        [SerializeField] private Button button;
        [SerializeField] private TrainingToneView<T> view;
        
        public Action<T> onUnlock = tone => {};
        
        private T _tone;
        private bool _expEnough;
        private bool _locked;

        private void Start()
        {
            button.onClick.AddListener(ShowInfo);
        }

        /// <summary>
        /// Устанавливает данные карточки
        /// </summary>
        public void Setup(int index, T tone)
        {
            _tone = tone;
            
            string nameKey = tone.GetDescription();
            toneName.text = LocalizationManager.Instance.Get(nameKey);

            name = $"ToneCard-{index + 1}";
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Обновляет состояние карточки
        /// </summary>
        public void Refresh(bool expEnough)
        {
            _expEnough = expEnough;

            object tone = _tone;
            if (typeof(T) == typeof(Themes))
                _locked = !PlayerManager.Data.Themes.Contains((Themes) tone);
            else
                _locked = !PlayerManager.Data.Styles.Contains((Styles) tone);
            
            button.image.color = _locked ? Color.white : Color.gray;
        }

        /// <summary>
        /// Показывает информацию о стилистике
        /// </summary>
        private void ShowInfo()
        {
            view.Show(_tone, _expEnough, _locked, onUnlock);
        }
    }
}