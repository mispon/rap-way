using Core;
using Data;
using Enums;
using Firebase.Analytics;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        [SerializeField] private WorkPoints labelWorkPoints;
        [SerializeField] private WorkPoints directorWorkPoints;
        [SerializeField] private WorkPoints operatorWorkPoints;
        [Space]
        [SerializeField] private Image labelAvatar;
        [SerializeField] private GameObject labelFrozen;
        
        [Header("Страница результата")] 
        [SerializeField] private ClipResultPage clipResult;

        private ClipInfo _clip;
        private LabelInfo _label;

        /// <summary>
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.CreateClipClick);
            
            _clip = (ClipInfo) args[0];
            Open();
            RefreshWorkAnims();
        }

        /// <summary>
        /// Работа, выполняемая за один день
        /// </summary>
        protected override void DoDayWork()
        {
            SoundManager.Instance.PlaySound(UIActionType.WorkPoint);
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
            GameEventsManager.Instance.CallEvent(GameEventType.Clip, ShowResultPage);
        }

        /// <summary>
        /// Возвращает длительность действия
        /// </summary>
        protected override int GetDuration()
        {
            return settings.ClipWorkDuration;
        }

        /// <summary>
        /// Генерирует очки работы
        /// </summary>
        private void GenerateWorkPoints()
        {
            var directorPointsValue = Random.Range(1, _clip.DirectorSkill + 2);
            _clip.DirectorPoints += directorPointsValue;
            directorWorkPoints.Show(directorPointsValue);

            var operatorPointsValue = Random.Range(1, _clip.OperatorSkill + 2);
            _clip.OperatorPoints += operatorPointsValue;
            operatorWorkPoints.Show(operatorPointsValue);

            var playerPointsValue = Random.Range(1, PlayerManager.Data.Stats.Charisma.Value + 2);
            playerWorkPoints.Show(playerPointsValue);

            if (Random.Range(0, 2) > 0)
                _clip.DirectorPoints += playerPointsValue;
            else
                _clip.OperatorPoints += playerPointsValue;
            
            var labelPoints = 0;
            if (_label is {IsFrozen: false})
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelWorkPoints.Show(labelPoints);
            }
            
            if (Random.Range(0, 2) > 0)
                _clip.DirectorPoints += labelPoints;
            else
                _clip.OperatorPoints += labelPoints;
        }

        /// <summary>
        /// Отображает количество сгенерированных очков работы
        /// </summary>
        private void DisplayWorkPoints()
        {
            directorPoints.text = _clip.DirectorPoints.ToString();
            operatorPoints.text = _clip.OperatorPoints.ToString();
        }

        protected override void BeforePageOpen()
        {
            base.BeforePageOpen();
            
            if (!string.IsNullOrEmpty(PlayerManager.Data.Label))
            {
                _label = LabelsManager.Instance.GetLabel(PlayerManager.Data.Label);
            }

            if (_label != null)
            {
                labelAvatar.gameObject.SetActive(true);
                labelAvatar.sprite = _label.Logo;
                labelFrozen.SetActive(_label.IsFrozen);
            } else
            {
                labelAvatar.gameObject.SetActive(false);
            }
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