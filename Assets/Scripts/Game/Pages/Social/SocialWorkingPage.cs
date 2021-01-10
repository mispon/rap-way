using Core;
using Data;
using Enums;
using Models.Info;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Social
{
    /// <summary>
    /// Страница работы социального действия
    /// </summary>
    public class SocialWorkingPage : BaseWorkingPage
    {
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text workPoints;
        [SerializeField] private WorkPoints playerWorkPoints;
        [SerializeField] private WorkPoints prManWorkPoints;
        [SerializeField] private Image prManAvatar;

        [Header("Страницы результата")]
        [SerializeField] private SocialResultPage[] socialResults;

        [Header("Данные")]
        [SerializeField] private ImagesBank imagesBank;
        
        private SocialInfo _social;
        private bool _hasPrMan;

        /// <summary>
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            _social = (SocialInfo) args[0];
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
        /// Генерирует очки работы
        /// </summary>
        private void GenerateWorkPoints()
        {
            var playerPointsValue = Random.Range(1, PlayerManager.Data.Stats.Charisma.Value + 1);
            playerWorkPoints.Show(playerPointsValue);

            var prManPointsValue = 0;
            if (_hasPrMan)
            {
                prManPointsValue = Random.Range(1, PlayerManager.Data.Team.PrMan.Skill.Value + 1);
                prManWorkPoints.Show(prManPointsValue);
            }
            
            _social.WorkPoints += playerPointsValue + prManPointsValue;
        }

        /// <summary>
        /// Отображает количество сгенерированных очков работы
        /// </summary>
        private void DisplayWorkPoints()
        {
            workPoints.text = _social.WorkPoints.ToString();
        }
        
        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected override void FinishWork()
        {
            GetPage(_social.Type).ShowPage(_social);
            Close();
        }
        
        /// <summary>
        /// Возвращает страницу для указанного типа соц. события 
        /// </summary>
        private SocialResultPage GetPage(SocialType type) => socialResults[(int) type];

        protected override void BeforePageOpen()
        {
            _hasPrMan = TeamManager.IsAvailable(TeammateType.PrMan);
            prManAvatar.sprite = _hasPrMan ? imagesBank.PrManActive : imagesBank.PrManInactive;
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();

            workPoints.text = "0";
            _social = null;
        }
    }
}

