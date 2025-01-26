using Core;
using Core.Localization;
using TMPro;
using UI.Base;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.Tutorial
{
    public class TutorialWindow : CanvasUIElement
    {
        [Header("Player")]
        [SerializeField] private TextMeshProUGUI nickname;
        [SerializeField] private TextMeshProUGUI realname;
        [SerializeField] private Image           playerIcon;

        [Header("Controls")]
        [SerializeField] private Text info;
        [SerializeField] private Button[] gameButtons;

        private readonly CompositeDisposable _disposable = new();

        protected override void AfterShow(object ctx = null)
        {
            foreach (var gameBtn in gameButtons)
            {
                gameBtn.OnClickAsObservable()
                    .Subscribe(e => Hide())
                    .AddTo(_disposable);
            }
        }

        public void ShowTutorial(TutorialStageSettings stageSettings)
        {
            var playerInfo = PlayerAPI.Data.Info;

            nickname.text     = playerInfo.NickName;
            realname.text     = $"{playerInfo.FirstName} {playerInfo.LastName}, {PlayerAPI.State.GetLevel()} lvl";
            playerIcon.sprite = SpritesManager.Instance.GetPortrait(playerInfo.NickName);

            info.text = LocalizationManager.Instance.Get(stageSettings.Text);
            for (var i = 0; i < gameButtons.Length; i++)
            {
                gameButtons[i].interactable = stageSettings.ButtonsActivity[i];
            }

            Show();
        }

        protected override void AfterHide()
        {
            _disposable.Clear();
        }
    }
}