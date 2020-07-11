using System;
using Game.Pages.Training.Tabs;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Training
{
    /// <summary>
    /// Главная страница тренировок персонажа
    /// </summary>
    public class TrainingMainPage : Page
    {
        [Header("Контролы")]
        [SerializeField] private Button[] tabsButtons;
        [SerializeField] private TrainingTab[] tabs;

        [Header("Рабочая страница")]
        [SerializeField] private TrainingWorkingPage workingPage;

        private void Start()
        {
            for (int i = 0; i < tabsButtons.Length; i++)
            {
                int index = i;
                tabsButtons[index].onClick.AddListener(() => OpenTab(index));
                
                tabs[index].Init();
                tabs[index].onStartTraining += StartTraining;
            }
        }

        /// <summary>
        /// Запускает тренировку 
        /// </summary>
        private void StartTraining(int duration, Func<string> onFinish)
        {
            workingPage.StartTrainig(duration, onFinish);
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
            OpenTab(0);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < tabsButtons.Length; i++)
            {
                tabsButtons[i].onClick.RemoveAllListeners();
                tabs[i].onStartTraining -= StartTraining;
            }
        }
    }
}