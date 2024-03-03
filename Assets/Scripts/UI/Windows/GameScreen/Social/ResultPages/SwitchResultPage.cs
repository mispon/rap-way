using Game.Player;
using Models.Production;
using UI.Windows.Pages.Social;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Social.ResultPages
{
    public class SwitchResultPage : SocialResultPage
    {
        [Header("Контролы")]
        [SerializeField] private Text streamName;
        [Space]
        [SerializeField] protected Text likes;
        [SerializeField] protected Text hype;

        protected override void DisplayResult(SocialInfo info)
        {
            string nickName = PlayerManager.Data.Info.NickName;
            streamName.text = $"\"{info.MainText}\" by {nickName}";

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";
        }
    }
}