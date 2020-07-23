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
            var typeLocalization = LocalizationManager.Instance.Get(Social.Data.Type.GetDescription());
            headerText.text = $"Фонд \"{Social.ExternalText}\"";
            commentText.text = $"Сегодня, {TimeManager.Instance.DisplayNow}, фонд \"{Social.ExternalText}\" получил от {PlayerManager.Data.Info} " +
                               $"средства в размере {Social.CharityMoney}$. Нуждающиеся в этих деньгах навсегда останутся ему благодарны.\n\n"; 
            
            //marksText
            hypeIncomeText.text = $"Хайп: +{Social.HypeIncome}";
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