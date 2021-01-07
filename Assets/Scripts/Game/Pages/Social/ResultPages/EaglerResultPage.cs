using Models.Info;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Social.ResultPages
{
    /// <summary>
    /// Страница результата псевдо-твиттера
    /// </summary>
    public class EaglerResultPage: SocialResultPage
    {
        [Header("Контролы")]
        [SerializeField] private Text nickname;
        [SerializeField] protected Text message;
        [Space]
        [SerializeField] protected Text likes;
        [SerializeField] protected Text hype;

        /// <summary>
        /// Отображает результат твита
        /// </summary>
        protected override void DisplayResult(SocialInfo info)
        {
            nickname.text = PlayerManager.Data.Info.NickName;
            message.text = info.MainText;

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";
        }
    }
}