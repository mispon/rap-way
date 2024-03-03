using Models.Production;
using UI.Windows.Pages.Social;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Social.ResultPages
{
    public class TackTackPage: SocialResultPage
    {
        private static readonly int Type = Animator.StringToHash("Type");
        
        [Header("Контролы")]
        [SerializeField] private Animator animator;
        [SerializeField] private Text comment;
        [Space]
        [SerializeField] protected Text likes;
        [SerializeField] protected Text hype;

        protected override void DisplayResult(SocialInfo info)
        {
            animator.SetInteger(Type, info.ModeIndex);
            comment.text = info.MainText;

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";
        }
    }
}