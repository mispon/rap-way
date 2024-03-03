using Enums;
using Firebase.Analytics;
using Game.Player;
using Game.Socials.Eagler;
using Game.Time;
using Models.Production;
using UI.Windows.Pages.Social;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Social.ResultPages
{
    public class EaglerResultPage: SocialResultPage
    {
        [Header("Контролы")]
        [SerializeField] private Text nickname;
        [SerializeField] protected Text message;
        [SerializeField] protected Text date;
        [Space]
        [SerializeField] protected Text likes;
        [SerializeField] protected Text hype;
   
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