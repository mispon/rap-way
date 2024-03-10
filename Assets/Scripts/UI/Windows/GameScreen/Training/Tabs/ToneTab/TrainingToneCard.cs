using System;
using Core;
using Core.Localization;
using Extensions;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Training.Tabs.ToneTab
{
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

        public void Setup(int index, TonesInfo info)
        {
            _info = info;

            string nameKey = GetValue().GetDescription();
            toneName.text = LocalizationManager.Instance.Get(nameKey).ToUpper();

            name = $"ToneCard-{index + 1}";
            gameObject.SetActive(true);
        }
        
        protected abstract Enum GetValue();
        
        protected abstract bool IsLocked();
        
        public void Refresh()
        {
            _expEnough = PlayerAPI.Data.Exp >= _info.Price;
            _locked = IsLocked();

            tonePrice.text = _locked ? _info.Price.ToString() : "";
            button.image.sprite = _locked ? _info.Locked : _info.Normal;
        }

        private void ShowInfo()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
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