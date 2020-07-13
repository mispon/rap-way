using System.Linq;
using Core;
using Enums;
using Game.Pages.Social.SocialStructs;
using Game.UI;
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
    public class SocialWorkingPage : Page
    {
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text header;
        [SerializeField] private ProgressBar progressBar;
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
        private SocialResultPage SocialResult(SocialType type) => socialResults.First(sr => sr.Type == type).Page;

        public void ShowPage(SocialInfo social)
        {
            _social = social;
            Open();
        }

        /// <summary>
        /// Обработчик истечения дня
        /// </summary>
        private void OnDayLeft()
        {
            if (progressBar.IsFinish)
                return;

            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        /// <summary>
        /// Генерирует очки работы
        /// </summary>
        private void GenerateWorkPoints()
        {
            var playerPointsValue = Random.Range(1, PlayerManager.Data.Stats.Charisma);
            _social.PlayerPoints += playerPointsValue;
            playerWorkPoints.Show(playerPointsValue);

            var prManPointsValue = 0;
            if (prMan.activeSelf)
            {
                prManPointsValue = Random.Range(1, PlayerManager.Data.Team.PrMan.Skill);
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
        /// Обработчик завершения социального действия
        /// </summary>
        private void FinishSocial()
        {
            SocialResult(_social.Data.Type).ShowPage(_social);
            Close();
        }

        #region PAGE CALLBACKS

        protected override void BeforePageOpen()
        {
            var typeDescription = LocalizationManager.Instance.Get(_social.Data.Type.GetDescription()); 
            header.text = $"{_social.Data.WorkingPageHeader} {typeDescription}";
            
            var active = !PlayerManager.Data.Team.PrMan.IsEmpty;
            prMan.SetActive(active);
            prManBucket.SetActive(active);
        }

        protected override void AfterPageOpen()
        {
            TimeManager.Instance.onDayLeft += OnDayLeft;
            TimeManager.Instance.SetActionMode();

            progressBar.Init(TimeManager.Instance.SecondsPerTick * _social.Data.Duration);
            progressBar.onFinish += FinishSocial;
            progressBar.Run();
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
            TimeManager.Instance.ResetActionMode();

            playerPoints.text = "0";
            prManPoints.text = "0";
            
            progressBar.onFinish -= FinishSocial;
            
            header.text = "";
            _social = null;
        }

        #endregion
    }
}

