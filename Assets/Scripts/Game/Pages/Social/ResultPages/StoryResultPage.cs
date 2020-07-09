using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Social.ResultPages
{
    public class StoryResultPage: SocialResultPage
    {
        [Header("Контролы")] 
        [SerializeField] protected Text headerText;
        [SerializeField] protected Text messageText;

        [Header("Контролы анализатора")] 
        [SerializeField] protected Text marksText;
        [SerializeField] protected Text hypeIncomeText;
        
        protected override void DisplayResult ()
        {
            var typeLocalization = LocalizationManager.Instance.Get(_social.Data.Type.GetDescription());
            headerText.text = $"Новый {typeLocalization}";
            messageText.text = _social.ExternalText;
            
            //marksText
            hypeIncomeText.text = $"Хайп: +{_social.HypeIncome}";
        }
        
        protected override void AfterPageClose()
        {
            base.AfterPageClose();

            headerText.text = "";
            messageText.text = "";
            //marksText.text = "";
            hypeIncomeText.text = "";
        }
    }
}