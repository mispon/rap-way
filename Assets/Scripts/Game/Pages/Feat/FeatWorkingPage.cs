using Core;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Pages.Feat
{
    /// <summary>
    /// Страница работы над фитом
    /// </summary>
    public class FeatWorkingPage : BaseWorkingPage
    {
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text bitPoints;
        [SerializeField] private Text textPoints;

        [Header("Команда игрока")]
        [SerializeField] private WorkPoints playerBitWorkPoints;
        [SerializeField] private WorkPoints playerTextWorkPoints;
        [SerializeField] private WorkPoints rapperBitWorkPoints;
        [SerializeField] private WorkPoints rapperTextWorkPoints;
        [SerializeField] private Image rapperAvatar;
        [SerializeField] private Sprite customRapperAvatar;

        [Header("Страница результата")]
        [SerializeField] private FeatResultPage featResult;
        
        private TrackInfo _track;
        
        /// <summary>
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            _track = (TrackInfo) args[0];
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
        /// Генерирует очки работы над треком
        /// </summary>
        private void GenerateWorkPoints()
        {
            var stats = PlayerManager.Data.Stats;
            
            var bitWorkPoints = CreateWorkPoints(
                stats.Bitmaking.Value, playerBitWorkPoints,
                _track.Feat.Vocobulary, rapperBitWorkPoints
            );
            var textWorkPoints = CreateWorkPoints(
                stats.Vocobulary.Value, playerTextWorkPoints,
                _track.Feat.Bitmaking, rapperTextWorkPoints
            );
            
            _track.BitPoints += bitWorkPoints;
            _track.TextPoints += textWorkPoints;
        }

        /// <summary>
        /// Создает рандомное количество очков работы в зависимости от скилла персонажа и тиммейта
        /// </summary>
        private static int CreateWorkPoints(
            int playerSkill, WorkPoints playerPoints,
            int rapperSkill, WorkPoints rapperPoints
        ) {
            var playerValue = Random.Range(1, playerSkill + 2);
            playerPoints.Show(playerValue);

            var rapperValue = Random.Range(1, rapperSkill + 2);
            rapperPoints.Show(rapperValue);

            return playerValue + rapperValue;
        }
        
        /// <summary>
        /// Обновляет значение рабочих очков в интерфейсе
        /// </summary>
        private void DisplayWorkPoints()
        {
            bitPoints.text = _track.BitPoints.ToString();
            textPoints.text = _track.TextPoints.ToString();
        }

        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected override void FinishWork()
        {
            featResult.Show(_track);
            Close();
        }

        /// <summary>
        /// Возвращает длительность действия
        /// </summary>
        protected override int GetDuration()
        {
            return settings.FeatWorkDuration;
        }
        
        protected override void BeforePageOpen()
        {
            base.BeforePageOpen();
            
            bitPoints.text = textPoints.text = "0";
            rapperAvatar.sprite = _track.Feat.IsCustom ? customRapperAvatar : _track.Feat.Avatar;
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();
            _track = null;
        }
    }
}