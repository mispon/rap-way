using Core;
using Data;
using Game.UI;
using Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Страница переговоров с реальным исполнителем по поводу фита или баттла
    /// </summary>
    public class RapperWorkingPage : Page
    {
        [Header("Настройки")]
        [SerializeField] private int duration;

        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text header;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private Text managementPoints;

        [Header("Персонажи")]
        [SerializeField] private Image rapperAvatar;
        [SerializeField] private WorkPoints playerWorkPoints;
        [SerializeField] private WorkPoints managerWorkPoints;

        [Header("Страница результата")]
        [SerializeField] private RapperResultPage rapperResult;

        private RapperInfo _rapper;
        private bool _isFeat;
        private int _managementPoints;
        
        /// <summary>
        /// Начинает переговоры с репером
        /// </summary>
        public void StartConversation(RapperInfo rapper, bool isFeat)
        {
            _rapper = rapper;
            _isFeat = isFeat;
            
            Open();
        }

        /// <summary>
        /// Вызывается по истечении игрового дня
        /// </summary>
        private void OnDayLeft()
        {
            if (progressBar.IsFinish)
                return;

            GenerateWorkPoints();
        }

        /// <summary>
        /// Генерирует очки работы
        /// </summary>
        private void GenerateWorkPoints()
        {
            int playerPoints = Random.Range(1, PlayerManager.Data.Stats.Management + 1);
            playerWorkPoints.Show(playerPoints);
            
            int managerPoints = Random.Range(1, PlayerManager.Data.Team.Manager.Skill + 1);
            managerWorkPoints.Show(managerPoints);

            _managementPoints += playerPoints + managerPoints;
            managementPoints.text = $"{_managementPoints}";
        }

        /// <summary>
        /// Вызывается при завершении переговоров
        /// </summary>
        private void FinishWork()
        {
            rapperResult.Show(_rapper, _managementPoints, _isFeat);
            Close();
        }

        protected override void BeforePageOpen()
        {
            header.text = $"{LocalizationManager.Instance.Get("conversation_with")} {_rapper.Name}";
            rapperAvatar.sprite = _rapper.Avatar;
            managementPoints.text = "0";
            _managementPoints = 0;
        }

        protected override void AfterPageOpen()
        {
            TimeManager.Instance.onDayLeft += OnDayLeft;
            TimeManager.Instance.SetActionMode();
            
            progressBar.Init(duration);
            progressBar.onFinish += FinishWork;
            progressBar.Run();
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
            TimeManager.Instance.ResetActionMode();
            
            progressBar.onFinish -= FinishWork;
            _rapper = null;
        }
    }
}