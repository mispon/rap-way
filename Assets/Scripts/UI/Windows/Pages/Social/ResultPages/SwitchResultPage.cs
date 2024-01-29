using Game.Player;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Social.ResultPages
{
    /// <summary>
    /// Страница результатов псевдо-твича
    /// </summary>
    public class SwitchResultPage : SocialResultPage
    {
        [Header("Контролы")]
        [SerializeField] private Text streamName;
        [Space]
        [SerializeField] protected Text likes;
        [SerializeField] protected Text hype;

        /// <summary>
        /// Отображает результаты игрового стрима
        /// </summary>
        protected override void DisplayResult(SocialInfo info)
        {
            string nickName = PlayerManager.Data.Info.NickName;
            streamName.text = $"\"{info.MainText}\" by {nickName}";

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";
        }
    }
}