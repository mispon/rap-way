using Models.Info;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Social.ResultPages
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

            string fondPart = noFondName ? "НЕКИЙ ФОНД" : $"ФОНД <color=#00fff4>«{info.AdditionalText}»</color>";
            string messagePart = noMessage ? "БЕЗ СООБЩЕНИЯ" : $"С СООБЩЕНИЕМ: <color=#00fff4>«{info.MainText}»</color>";
            string moneyPart = $"<color=#00F475>{info.CharityAmount.GetMoney()}</color>";

            message.text = $"СЕГОДНЯ {fondPart} ПОЛУЧИЛ {moneyPart} {messagePart}";
            hype.text = $"+{info.HypeIncome}";
        }
    }
}