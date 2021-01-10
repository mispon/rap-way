using Core;
using Enums;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Clip
{
    /// <summary>
    /// Страница работы над клипом
    /// </summary>
    public class ClipWorkingPage : BaseWorkingPage
    {
        [SerializeField] private Text directorPoints;
        [SerializeField] private Text operatorPoints;

        [Header("Команда игрока")] 
        [SerializeField] private WorkPoints playerWorkPoints;

        [SerializeField] private WorkPoints directorWorkPoints;
        [SerializeField] private WorkPoints operatorWorkPoints;

        [Header("Страница результата")] 
        [SerializeField] private ClipResultPage clipResult;

        private ClipInfo _clip;

        /// <summary>
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            _clip = (ClipInfo) args[0];
            Open();
            RefreshWorkAnims();
        }

        /// <summary>
        /// Работа, выполняемая за один день
        /// </summary>
        protected override void DoDayWork()
        {
            SoundManager.Instance.PlayWorkPoint();
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        /// <summary>
        /// Обработчик перехода к странице результата
        /// </summary>
        private void ShowResultPage()
        {
            clipResult.Show(_clip);
            Close();
        }

        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected override void FinishWork()
        {
            GameEventsManager.CallEvent(GameEventType.Clip, ShowResultPage);
        }

        /// <summary>
        /// Генерирует очки работы
        /// </summary>
        private void GenerateWorkPoints()
        {
            var directorPointsValue = Random.Range(1, _clip.DirectorSkill + 1);
            _clip.DirectorPoints += directorPointsValue;
            directorWorkPoints.Show(directorPointsValue);

            var operatorPointsValue = Random.Range(1, _clip.OperatorSkill + 1);
            _clip.OperatorPoints += operatorPointsValue;
            operatorWorkPoints.Show(operatorPointsValue);

            var playerPointsValue = Random.Range(1, PlayerManager.Data.Stats.Charisma.Value + 1);
            playerWorkPoints.Show(playerPointsValue);

            if (Random.Range(0, 2) > 0)
                _clip.DirectorPoints += playerPointsValue;
            else
                _clip.OperatorPoints += playerPointsValue;
        }

        /// <summary>
        /// Отображает количество сгенерированных очков работы
        /// </summary>
        private void DisplayWorkPoints()
        {
            directorPoints.text = _clip.DirectorPoints.ToString();
            operatorPoints.text = _clip.OperatorPoints.ToString();
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();
            
            directorPoints.text = "0";
            operatorPoints.text = "0";

            _clip = null;
        }
    }
}