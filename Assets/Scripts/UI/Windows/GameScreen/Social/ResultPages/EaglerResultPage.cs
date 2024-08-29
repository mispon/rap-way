using Enums;
using Game.SocialNetworks.Eagler;
using Game.Time;
using Core.Analytics;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Social.ResultPages
{
    public class EaglerResultPage : SocialResultPage
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
            AnalyticsManager.LogEvent(FirebaseGameEvents.TwitPublished);

            var nickName = PlayerAPI.Data.Info.NickName;

            nickname.text = nickName;
            date.text = TimeManager.Instance.DisplayNow;
            message.text = info.MainText;

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";

            EaglerManager.Instance.CreateUserEagle(nickName, info.MainText, info.Likes);
        }
    }
}