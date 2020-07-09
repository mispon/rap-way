using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Social.ResultPages
{
    public class TranslationResultPage: SocialResultPage
    {
        [Header("Контролы")] 
        [SerializeField] private Text headerText;
        
        [Header("Контролы анализатора")] 
        [SerializeField] private Text marksText;
        [SerializeField] private Text hypeIncomeText;
        
        protected override void DisplayResult ()
        {
            var typeLocalization = LocalizationManager.Instance.Get(_social.Data.Type.GetDescription());
            var nickname = PlayerManager.Data.Info.NickName;
            headerText.text = $"{typeLocalization} <{nickname}>:\n{_social.ExternalText}";
            
            //marksText
            hypeIncomeText.text = $"Хайп: +{_social.HypeIncome}";
        }
        
        protected override void AfterPageClose()
        {
            base.AfterPageClose();

            headerText.text = "";
            //marksText.text = "";
            hypeIncomeText.text = "";
        }
    }
}