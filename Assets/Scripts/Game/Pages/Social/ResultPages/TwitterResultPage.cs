using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Social.ResultPages
{
    public class TwitterResultPage: SocialResultPage
    {
        [Header("Контролы")] 
        [SerializeField] protected Text headerText;
        [SerializeField] private Text nicknameText;
        [SerializeField] protected Text twitText;

        [Header("Контролы анализатора")] 
        [SerializeField] protected Text marksText;
        [SerializeField] protected Text hypeIncomeText;
        
        protected override void DisplayResult ()
        {
            var typeLocalization = LocalizationManager.Instance.Get(Social.Data.Type.GetDescription());
            headerText.text = $"Новый {typeLocalization}";
            nicknameText.text = $"{PlayerManager.Data.Info.NickName}:";
            twitText.text = Social.ExternalText;
            
            //marksText
            hypeIncomeText.text = $"Хайп: +{Social.HypeIncome}";
        }

        protected override void AfterPageClose()
        {
            base.AfterPageClose();

            headerText.text = "";
            nicknameText.text = "";
            twitText.text = "";
            //marksText.text = "";
            hypeIncomeText.text = "";
        }
    }
}