using Enums;
using Extensions;
using Game;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Social.Tabs
{
    public class CharityTab : BaseSocialsTab
    {
        [SerializeField] private InputField messageInput;
        [SerializeField] private InputField fondInput;
        [SerializeField] private Text balance;
        [Space]
        [SerializeField] private Slider amountSlider;
        [SerializeField] private Text amountLabel;

        private int _amount;

        protected override void TabStart()
        {
            amountSlider.onValueChanged.AddListener(SetAmount);
        }
        
        private void SetAmount(float value)
        {
            _amount = (int) value;
            amountLabel.text = _amount.GetMoney();
        }
        
        protected override SocialInfo GetInfo()
        {
            return new SocialInfo
            {
                Type = SocialType.Charity,
                MainText = messageInput.text,
                AdditionalText = fondInput.text,
                CharityAmount = _amount
            };
        }
        
        protected override void OnOpen()
        {
            SetSliderBorders();

            SetAmount(amountSlider.minValue);
            balance.text = $"{PlayerAPI.Data.Money.GetMoney()}";
            fondInput.text = string.Empty;
            messageInput.text = string.Empty;
            
            base.OnOpen();
        }

        protected override bool CheckStartConditions()
        {
            var settings = GameManager.Instance.Settings.Socials;
            
            bool noCooldown = base.CheckStartConditions();
            return noCooldown && PlayerAPI.Data.Money >= settings.MinBalanceForCharity;
        }

        private void SetSliderBorders()
        {
            int money = PlayerAPI.Data.Money;

            int min = 0;
            int max = 0;

            var settings = GameManager.Instance.Settings.Socials;
            if (money >= settings.MinBalanceForCharity)
            {
                min = Mathf.Max(1, money / 100);
                max = min * 10;
            }

            amountSlider.minValue = min;
            amountSlider.maxValue = max;
        }
    }
}