using Core.Localization;
using Enums;
using ScriptableObjects;
using UI.Base;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.Tutorial
{
    public class TutorialWindow : CanvasUIElement
    {
        [SerializeField] private ImagesBank imagesBank;

        [Header("Player")]
        [SerializeField] private Text nickname;
        [SerializeField] private Image playerIcon;

        [Header("Controls")]
        [SerializeField] private Text info;
        [SerializeField] private Button[] gameButtons;
        [SerializeField] private Button skipButton;

        private readonly CompositeDisposable _disposable = new();

        private void Start()
        {
            skipButton.onClick.AddListener(() => base.Hide());
        }

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

            nickname.text = playerInfo.NickName;
            playerIcon.sprite = playerInfo.Gender == Gender.Male
                ? imagesBank.MaleAvatar
                : imagesBank.FemaleAvatar;

            info.text = LocalizationManager.Instance.Get(stageSettings.Text);
            for (int i = 0; i < gameButtons.Length; i++)
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