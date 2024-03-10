using Core.Localization;
using Enums;
using ScriptableObjects;
using Sirenix.OdinInspector;
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
        
        [BoxGroup("Player"), SerializeField] private Text nickname;
        [BoxGroup("Player"), SerializeField] private Image playerIcon;
        
        [BoxGroup("Controls"), SerializeField] private Text info;
        [BoxGroup("Controls"), SerializeField] private Button[] gameButtons;

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