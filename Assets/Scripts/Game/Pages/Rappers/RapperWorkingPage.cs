using Data;
using Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Страница переговоров с реальным исполнителем по поводу фита или баттла
    /// </summary>
    public class RapperWorkingPage : BaseWorkingPage
    {
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text header;
        [SerializeField] private Text managementPoints;

        [Header("Персонажи")]
        [SerializeField] private Image rapperAvatar;
        [SerializeField] private WorkPoints playerWorkPoints;
        [SerializeField] private WorkPoints managerWorkPoints;

        [Header("Страница результата")]
        [SerializeField] private RapperResultPage rapperResult;

        private RapperInfo _rapper;
        private bool _isFeat;
        private int _playerPoints;
        private int _rapperPoints;

        /// <summary>
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            _rapper = (RapperInfo) args[0]; 
            _isFeat = (bool) args[1];
            
            Open();
        }

        /// <summary>
        /// Работа, выполняемая за один день
        /// </summary>
        protected override void DoDayWork()
        {
            GenerateWorkPoints();
        }
        
        /// <summary>
        /// Вызывается при завершении переговоров
        /// </summary>
        protected override void FinishWork()
        {
            rapperResult.Show(_rapper, _playerPoints, _rapperPoints, _isFeat);
            Close();
        }

        /// <summary>
        /// Генерирует очки работы
        /// </summary>
        private void GenerateWorkPoints()
        {
            int playerPoints = Random.Range(1, PlayerManager.Data.Stats.Management.Value + 1);
            playerWorkPoints.Show(playerPoints);
            
            int managerPoints = Random.Range(1, PlayerManager.Data.Team.Manager.Skill.Value + 1);
            managerWorkPoints.Show(managerPoints);

            _playerPoints += playerPoints + managerPoints;
            managementPoints.text = $"{_playerPoints}";

            _rapperPoints += Random.Range(1, _rapper.Management + 1);
        }

        protected override void BeforePageOpen()
        {
            header.text = $"{LocalizationManager.Instance.Get("conversation_with")} {_rapper.Name}";
            rapperAvatar.sprite = _rapper.Avatar;
            managementPoints.text = "0";
            _playerPoints = 0;
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();
            _rapper = null;
        }
    }
}