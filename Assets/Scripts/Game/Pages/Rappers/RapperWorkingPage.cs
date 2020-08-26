using Data;
using Enums;
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
        [SerializeField] private Text managementPointsLabel;
        [SerializeField] private Text rapperPointsLabel;

        [Header("Персонажи")]
        [SerializeField] private Image managerAvatar;
        [SerializeField] private Image rapperAvatar;
        [SerializeField] private WorkPoints playerWorkPoints;
        [SerializeField] private WorkPoints managerWorkPoints;
        [SerializeField] private WorkPoints rapperWorkPoints;

        [Header("Страница результата")]
        [SerializeField] private RapperResultPage rapperResult;

        [Header("Даннык")]
        [SerializeField] private ImagesBank imagesBank;

        private RapperInfo _rapper;
        private bool _isFeat;
        private int _playerPoints;
        private int _rapperPoints;
        private bool _hasManager;

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
            GeneratePlayerWorkPoints();
            GenerateRapperWorkPoints();
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
        /// Генерирует очки работы игрока
        /// </summary>
        private void GeneratePlayerWorkPoints()
        {
            int playerPoints = Random.Range(1, PlayerManager.Data.Stats.Management.Value + 1);
            playerWorkPoints.Show(playerPoints);

            int managerPoints = 0;
            if (_hasManager)
            {
                managerPoints = Random.Range(1, PlayerManager.Data.Team.Manager.Skill.Value + 1);
                managerWorkPoints.Show(managerPoints);
            }

            _playerPoints += playerPoints + managerPoints;
            managementPointsLabel.text = $"{_playerPoints}";
        }

        /// <summary>
        /// Генерирует очки работы игрока
        /// </summary>
        private void GenerateRapperWorkPoints()
        {
            int rapperPoints =  Random.Range(1, _rapper.Management + 1);
            rapperWorkPoints.Show(rapperPoints);
            _rapperPoints += rapperPoints;
            rapperPointsLabel.text = $"{_rapperPoints}";
        }

        protected override void BeforePageOpen()
        {
            _hasManager = TeamManager.IsAvailable(TeammateType.Manager);
            managerAvatar.sprite = _hasManager ? imagesBank.ProducerActive : imagesBank.ProducerInactive;
            
            rapperAvatar.sprite = _rapper.Avatar;
            managementPointsLabel.text = "0";
            rapperPointsLabel.text = "0";
            _playerPoints = 0;
            _rapperPoints = 0;
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();
            _rapper = null;
        }
    }
}