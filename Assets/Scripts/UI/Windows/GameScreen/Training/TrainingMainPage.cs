using System;
using Core.Context;
using Game.Player;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Controls.Carousel;
using UI.Windows.GameScreen.Training.Tabs;
using UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Training
{
    public class TrainingMainPage : Page
    {
        [Header("Контролы")]
        [SerializeField] private Carousel tabsCarousel;
        [SerializeField] private TrainingTab[] tabs;
        [SerializeField] private Text expLabel;

        private int _tabIndex;

        private void Start()
        {
            tabsCarousel.onChange += OnTabChanged;

            foreach (var tab in tabs)
            {
                tab.Init();
                tab.onStartTraining += ApplyTraining;
            }
        }
        
        public override void Show(object ctx = null)
        {
            _tabIndex = ctx.Value<int>();
            
            base.Show(ctx);
            OpenTab(_tabIndex);
        }
        
        private void OnTabChanged(int index)
        {
            OpenTab(index);
        }

        private void ApplyTraining(Func<int> training)
        {
            int cost = training.Invoke();
            PlayerManager.Data.Exp -= cost;
            
            DisplayExp();
            RefreshTab();
        }

        private void OpenTab(int index)
        {
            _tabIndex = index;
            HintsManager.Instance.ShowHint($"tutorial_training_{index}");

            for (var i = 0; i < tabs.Length; i++)
            {
                var tab = tabs[i];
                tab.Toggle(i == index);
            }
        }

        private void RefreshTab()
        {
            OpenTab(_tabIndex);
        }
        
        private void DisplayExp() => expLabel.text =  PlayerManager.Data.Exp.ToString();

        protected override void BeforeShow()
        {
            DisplayExp();
        }

        protected override void AfterShow()
        {
            tabsCarousel.SetIndex(_tabIndex);
            HintsManager.Instance.ShowHint($"tutorial_training_{_tabIndex}");
        }

        protected override void AfterHide()
        {
            MsgBroker.Instance.Publish(new TutorialWindowControlMessage());
            tabsCarousel.SetIndex(0);
        }

        private void OnDestroy()
        {
            tabsCarousel.onChange -= OnTabChanged;
            foreach (var tab in tabs)
            {
                tab.onStartTraining -= ApplyTraining;
            }
        }
    }
}