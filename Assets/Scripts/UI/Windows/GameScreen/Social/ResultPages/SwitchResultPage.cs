using Models.Production;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

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
            string nickName = PlayerAPI.Data.Info.NickName;
            streamName.text = $"\"{info.MainText}\" by {nickName}";

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";
        }
    }
}