using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Social.ResultPages
{
    public class TelescopeResultPage: SocialResultPage
    {
        [Header("Контролы")]
        [SerializeField] private Text comment;
        [Space]
        [SerializeField] protected Text likes;
        [SerializeField] protected Text hype;

        protected override void DisplayResult(SocialInfo info)
        {
            comment.text = info.MainText;

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";
        }
    }
}