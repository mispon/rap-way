using Core.Localization;
using Enums;
using Game.Player;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Base;
using UI.Enums;
using UI.MessageBroker;
using UI.MessageBroker.Messages;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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
        
        protected override void SetupListenersOnShow()
        {
            gameButtons[0].OnClickAsObservable().
                Subscribe(e => OpenGameWindow(WindowType.ProductionTrackSettings)).
                AddTo(_disposable);
            gameButtons[2].OnClickAsObservable().
                Subscribe(e => OpenGameWindow(WindowType.Training)).
                AddTo(_disposable);
            gameButtons[3].OnClickAsObservable().
                Subscribe(e => OpenGameWindow(WindowType.Store)).
                AddTo(_disposable);
            gameButtons[4].OnClickAsObservable().
                Subscribe(e => OpenGameWindow(WindowType.Personal)).
                AddTo(_disposable);
        }

        public void ShowTutorial(TutorialStageSettings stageSettings)
        {
            var playerInfo = PlayerManager.Data.Info;
            
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

        private void OpenGameWindow(WindowType type)
        {
            UIMessageBroker.Instance.Publish(new WindowControlMessage {Type = type});
            Hide();
        }

        protected override void DisposeListeners()
        {
            _disposable.Clear();
        }
    }
}