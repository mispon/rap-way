using System;
using Core.Context;
using MessageBroker;
using MessageBroker.Messages.UI;
using Sirenix.OdinInspector;
using UI.Controls.Carousel;
using UI.Windows.GameScreen.Training.Tabs;
using UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Training
{
    public class TrainingMainPage : Page
    {
        [BoxGroup("Controls"), SerializeField] private Carousel tabsCarousel;
        [BoxGroup("Controls"), SerializeField] private TrainingTab[] tabs;
        [BoxGroup("Controls"), SerializeField] private Text expLabel;

        private int _tabIndex;

        public override void Initialize()
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
            PlayerAPI.Data.Exp -= cost;
            
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
        
        private void DisplayExp() => expLabel.text =  PlayerAPI.Data.Exp.ToString();

        protected override void BeforeShow(object ctx = null)
        {
            DisplayExp();
        }

        protected override void AfterShow(object ctx = null)
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