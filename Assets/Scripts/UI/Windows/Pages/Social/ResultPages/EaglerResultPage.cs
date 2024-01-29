using Enums;
using Firebase.Analytics;
using Game.Player;
using Game.Socials;
using Game.Time;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Social.ResultPages
{
    /// <summary>
    /// Страница результата псевдо-твиттера
    /// </summary>
    public class EaglerResultPage: SocialResultPage
    {
        [Header("Контролы")]
        [SerializeField] private Text nickname;
        [SerializeField] protected Text message;
        [SerializeField] protected Text date;
        [Space]
        [SerializeField] protected Text likes;
        [SerializeField] protected Text hype;

        /// <summary>
        /// Отображает результат твита
        /// </summary>
        protected override void DisplayResult(SocialInfo info)
        {
            var nn = PlayerManager.Data.Info.NickName;

            nickname.text = nn;
            date.text = TimeManager.Instance.DisplayNow;
            message.text = info.MainText;

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";

            EaglerManager.Instance.CreateUserEagle(nn, info.MainText, info.Likes);
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.TwitPublished);
        }
    }
}