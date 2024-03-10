using Enums;
using Firebase.Analytics;
using Game.Socials.Eagler;
using Game.Time;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

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
            var nickName = PlayerAPI.Data.Info.NickName;

            nickname.text = nickName;
            date.text = TimeManager.Instance.DisplayNow;
            message.text = info.MainText;

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";

            EaglerManager.Instance.CreateUserEagle(nickName, info.MainText, info.Likes);
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.TwitPublished);
        }
    }
}