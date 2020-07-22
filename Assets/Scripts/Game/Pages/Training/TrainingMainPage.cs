using System;
using Game.Pages.Training.Tabs;
using UnityEngine;
using Utils;

namespace Game.Pages.Training
{
    /// <summary>
    /// Главная страница тренировок персонажа
    /// </summary>
    public class TrainingMainPage : Page
    {
        [Header("Контролы")]
        [SerializeField] private Switcher tabsSwitcher;
        [SerializeField] private TrainingTab[] tabs;

        [Header("Рабочая страница")]
        [SerializeField] private TrainingWorkingPage workingPage;

        private void Start()
        {
            tabsSwitcher.onIndexChange += OnTabChanged;

            foreach (var tab in tabs)
            {
                tab.Init();
                tab.onStartTraining += StartTraining;
            }
        }

        /// <summary>
        /// Открывает страницу и указанную вкладку
        /// </summary>
        public void OpenPage(int tabIndex)
        {
            Open();
            OpenTab(tabIndex);
        }

        /// <summary>
        /// Обработчик изменения индекса вкладки 
        /// </summary>
        private void OnTabChanged(int index)
        {
            OpenTab(index);
        }

        /// <summary>
        /// Запускает тренировку 
        /// </summary>
        private void StartTraining(int duration, Func<string> onFinish)
        {
            workingPage.StartWork(duration, onFinish);
            Close();
        }

        /// <summary>
        /// Открывает вкладку по индексу 
        /// </summary>
        private void OpenTab(int index)
        {
            for (var i = 0; i < tabs.Length; i++)
            {
                var tab = tabs[i];
                tab.Toggle(i == index);
            }
        }

        protected override void BeforePageOpen()
        {
            // TODO: Localization
            tabsSwitcher.InstantiateElements(new [] {"Навыки", "Умения", "Стили", "Команда"});
            OpenTab(0);
        }

        private void OnDestroy()
        {
            tabsSwitcher.onIndexChange -= OnTabChanged;
            
            foreach (var tab in tabs)
            {
                tab.onStartTraining -= StartTraining;
            }
        }
    }
}