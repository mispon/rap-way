using System.Linq;
using Enums;
using Game.Pages.Social.SocialStructs;
using Localization;
using Models.Info;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Social
{
    /// <summary>
    /// Страница работы социального действия
    /// </summary>
    public class SocialWorkingPage : BaseWorkingPage
    {
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text header;
        [SerializeField] private Text playerPoints;
        [SerializeField] private Text prManPoints;
        [SerializeField] private GameObject prManBucket;
        
        [Header("Команда игрока")]
        [SerializeField] private WorkPoints playerWorkPoints;
        [SerializeField] private WorkPoints prManWorkPoints;
        [SerializeField] private GameObject prMan;

        [Header("Страница результата")]
        [SerializeField, ArrayElementTitle("Type")] private TypedResultPage[] socialResults; 
        
        private SocialInfo _social;

        /// <summary>
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            _social = (SocialInfo) args[0];
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
        /// Генерирует очки работы
        /// </summary>
        private void GenerateWorkPoints()
        {
            var playerPointsValue = Random.Range(1, PlayerManager.Data.Stats.Charisma.Value + 1);
            _social.PlayerPoints += playerPointsValue;
            playerWorkPoints.Show(playerPointsValue);

            var prManPointsValue = 0;
            if (prMan.activeSelf)
            {
                prManPointsValue = Random.Range(1, PlayerManager.Data.Team.PrMan.Skill.Value + 1);
                prManWorkPoints.Show(prManPointsValue);
            }
            _social.PrManPoints += prManPointsValue;
        }

        /// <summary>
        /// Отображает количество сгенерированных очков работы
        /// </summary>
        private void DisplayWorkPoints()
        {
            playerPoints.text = _social.PlayerPoints.ToString();
            if (prMan.activeSelf)
                prManPoints.text = _social.PrManPoints.ToString();
        }
        
        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected override void FinishWork()
        {
            GetPage(_social.Data.Type).ShowPage(_social);
            Close();
        }
        
        /// <summary>
        /// Возвращает страницу для указанного типа соц. события 
        /// </summary>
        private SocialResultPage GetPage(SocialType type)
        {
            return socialResults.First(sr => sr.Type == type).Page;
        }

        protected override void BeforePageOpen()
        {
            var typeDescription = LocalizationManager.Instance.Get(_social.Data.Type.GetDescription()); 
            header.text = $"{_social.Data.WorkingPageHeader} {typeDescription}";
            
            var active = !PlayerManager.Data.Team.PrMan.IsEmpty;
            prMan.SetActive(active);
            prManBucket.SetActive(active);
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();

            playerPoints.text = "0";
            prManPoints.text = "0";
            header.text = "";
            
            _social = null;
        }
    }
}

