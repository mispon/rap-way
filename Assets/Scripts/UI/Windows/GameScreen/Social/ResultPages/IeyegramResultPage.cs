using Models.Production;
using UI.Windows.Pages.Social;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Social.ResultPages
{
    public class IeyegramResultPage: SocialResultPage
    {
        [Header("Контролы")]
        [SerializeField] private Image picture;
        [SerializeField] private Text comment;
        [Space]
        [SerializeField] protected Text likes;
        [SerializeField] protected Text hype;

        [Header("Данные")]
        [SerializeField] private Sprite[] pics;

        protected override void DisplayResult(SocialInfo info)
        {
            picture.sprite = pics[info.ModeIndex];
            comment.text = info.MainText;

            likes.text = info.Likes.ToString();
            hype.text = $"+{info.HypeIncome}";
        }
    }
}