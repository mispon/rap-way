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
            message.text = @$"СЕГОДНЯ ФОНД <color=#00fff4>«{info.AdditionalText}»</color>
                             ПОЛУЧИЛ {info.CharityAmount.GetDisplay()}$
                             С СООБЩЕНИЕМ: <color=#00fff4>«{info.MainText}»</color>";
            hype.text = $"+{info.HypeIncome}";
        }
    }
}