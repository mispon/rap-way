using Extensions;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Social.ResultPages
{
    /// <summary>
    /// Страница результатов пожертвования
    /// </summary>
    public class CharityResultPage: SocialResultPage
    {
        [Header("Контролы")]
        [SerializeField] private Text message;
        [SerializeField] protected Text hype;

        /// <summary>
        /// Отображает результаты пожествования
        /// </summary>
        protected override void DisplayResult(SocialInfo info)
        {
            bool noFondName = string.IsNullOrEmpty(info.AdditionalText);
            bool noMessage = string.IsNullOrEmpty(info.MainText);

            string fondPart = noFondName
                ? GetLocale("socials_some_fond")
                : GetLocale("socials_fond_name", info.AdditionalText);
            string messagePart = noMessage
                ? GetLocale("socials_fond_no_message")
                : GetLocale("socials_fond_with_message", info.MainText);

            string moneyPart = $"<color=#00F475>{info.CharityAmount.GetMoney()}</color>";

            message.text = GetLocale("socials_fond_result", fondPart, moneyPart, messagePart).ToUpper();
            hype.text = $"+{info.HypeIncome}";
        }
    }
}