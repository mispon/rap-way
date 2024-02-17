using System;
using UI.Base;
using UI.MessageBroker;
using UI.MessageBroker.Messages;
using UniRx;
using UnityEngine;

namespace UI.Windows.Tutorial
{
    [Serializable]
    public class TutorialStageSettings
    {
        public string Text;
        public bool[] ButtonsActivity;
    }
    
    public sealed class TutorialWindowContainer: UIElementContainer
    {
        [SerializeField] private TutorialWindow tutorialWindow;
        [SerializeField] private TutorialStageSettings[] tutorialStages;
        
        private const int NO_STAGES = -1;
        private const string TUTORIAL_STAGE_INDEX_KEY = "RapWay_TutorialStage";
        
        public override void Initialize()
        {
            tutorialWindow.Initialize();
            
            UIMessageBroker.Instance
                .Receive<TutorialWindowControlMessage>()
                .Subscribe(_ => ShowNextTutorialStage())
                .AddTo(disposables);
        }

        private void ShowNextTutorialStage()
        {
            int stageIndex = GetNextTutorialStageIndex();
            
            // player passed all tutorial stages
            if (stageIndex == NO_STAGES)
                return;

            var nextStage = tutorialStages[stageIndex];
            tutorialWindow.ShowTutorial(nextStage);
        }
        
        private int GetNextTutorialStageIndex()
        {
            int index = PlayerPrefs.HasKey(TUTORIAL_STAGE_INDEX_KEY) 
                ? PlayerPrefs.GetInt(TUTORIAL_STAGE_INDEX_KEY)
                : -1;
            
            if (index >= tutorialStages.Length-1)
            {
                return NO_STAGES;
            }

            index++;
            PlayerPrefs.SetInt(TUTORIAL_STAGE_INDEX_KEY, index);
            
            return index;
        }
        
        protected override void Deactivate()
        {
            base.Deactivate();
            tutorialWindow.Hide();
        }
    }
}