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
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text header;
        [SerializeField] private Text playerPoints;
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
            header.text = $"Работа над клипом трека \"{PlayerManager.GetTrackName(_clip.TrackId)}\"";
            Open();
        }

        /// <summary>
        /// Работа, выполняемая за один день
        /// </summary>
        protected override void DoDayWork()
        {
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected override void FinishWork()
        {
            clipResult.Show(_clip);
            Close();
        }

        /// <summary>
        /// Генерирует очки работы
        /// </summary>
        private void GenerateWorkPoints()
        {
            var playerPointsValue = Random.Range(1, PlayerManager.Data.Stats.Charisma.Value + 1);
            _clip.PlayerPoints += playerPointsValue;
            playerWorkPoints.Show(playerPointsValue);
            
            var directorPointsValue = Random.Range(1, _clip.DirectorSkill + 1);
            _clip.DirectorPoints += directorPointsValue;
            directorWorkPoints.Show(directorPointsValue);
            
            var operatorPointsValue = Random.Range(1, _clip.OperatorSkill + 1);
            _clip.OperatorPoints += operatorPointsValue;
            operatorWorkPoints.Show(operatorPointsValue);
        }

        /// <summary>
        /// Отображает количество сгенерированных очков работы
        /// </summary>
        private void DisplayWorkPoints()
        {
            playerPoints.text = _clip.PlayerPoints.ToString();
            directorPoints.text = _clip.DirectorPoints.ToString();
            operatorPoints.text = _clip.OperatorPoints.ToString();
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();

            playerPoints.text = "0";
            directorPoints.text = "0";
            operatorPoints.text = "0";
            
            _clip = null;
        }
    }
}