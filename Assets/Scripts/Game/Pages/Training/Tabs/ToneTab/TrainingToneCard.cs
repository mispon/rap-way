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
    public class TrainingToneCard : MonoBehaviour
    {
        [SerializeField] private Text toneName;
        [SerializeField] private Button button;
        [SerializeField] private TrainingToneView view;
        
        public Action<Enum> onUnlock = tone => {};
        
        private Enum _tone;
        private bool _expEnough;
        private bool _locked;

        private void Start()
        {
            button.onClick.AddListener(ShowInfo);
        }

        /// <summary>
        /// Устанавливает данные карточки
        /// </summary>
        public void Setup(int index, Enum tone)
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
            
            if (_tone.GetType() == typeof(Themes))
                _locked = !PlayerManager.Data.Themes.Contains((Themes) _tone);
            else
                _locked = !PlayerManager.Data.Styles.Contains((Styles) _tone);
            
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