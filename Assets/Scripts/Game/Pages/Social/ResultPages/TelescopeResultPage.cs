using Models.Info;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Social.ResultPages
{
    /// <summary>
    /// Страница результатов псевдо-перископа
    /// </summary>
    public class TelescopeResultPage: SocialResultPage
    {
        [Header("Контролы")]
        [SerializeField] private Text comment;
        [Space]
        [SerializeField] protected Text likes;
        [SerializeField] protected Text hype;

        /// <summary>
        /// Отображает результаты трансляции в соц. сети
        /// </summary>
        protected override void DisplayResult(SocialInfo info)
        {
            comment.text = info.MainText;

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";
        }
    }
}