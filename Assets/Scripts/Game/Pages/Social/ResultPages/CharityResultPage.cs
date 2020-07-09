using Core;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Social.ResultPages
{
    public class CharityResultPage: SocialResultPage
    {
        [Header("Контролы")] 
        [SerializeField] private Text headerText;
        [SerializeField] private Text commentText;

        [Header("Контролы анализатора")] 
        [SerializeField] private Text marksText;
        [SerializeField] private Text hypeIncomeText;

        protected override void DisplayResult ()
        {
            var typeLocalization = LocalizationManager.Instance.Get(_social.Data.Type.GetDescription());
            headerText.text = $"Фонд \"{_social.ExternalText}\"";
            commentText.text = $"Сегодня, {TimeManager.Instance.DisplayNow}, фонд \"{_social.ExternalText}\" получил от {PlayerManager.Data.Info} " +
                               $"средства в размере {_social.CharityMoney}$. Нуждающиеся в этих деньгах навсегда останутся ему благодарны.\n\n" +
                               $"P.S.: Не унывай! У тебя на счету осталось {PlayerManager.Data.Money}$>"; 
            
            //marksText
            hypeIncomeText.text = $"Хайп: +{_social.HypeIncome}";
        }
        
        protected override void AfterPageClose()
        {
            base.AfterPageClose();

            headerText.text = "";
            commentText.text = "";
            //marksText.text = "";
            hypeIncomeText.text = "";
        }
    }
}