using System.Linq;
using Enums;
using Models.Info.Production;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Concert
{
    /// <summary>
    /// Страница катсцены концерта
    /// </summary>
    public class ConcertCutscenePage : Page
    {
        private static readonly Color FlexingGraphicStartColor = new Color(1, 1, 1, 0);

        [Header("Персонаж")]
        [SerializeField] private Image maleCharacter;
        [SerializeField] private SkeletonGraphic femaleCharacter;
        
        [Header("Анимации флекса")] 
        [SerializeField] private SkeletonGraphic flexingGraphic;
        [SerializeField, SpineAnimation(dataField = "flexingGraphic")] 
        private string[] flexingStates; 
        
        [Header("Пропуск катсцены")] 
        [SerializeField] private Button skipButton;

        private ConcertInfo _concert;

        private void Start()
        {
            skipButton.onClick.AddListener(Close);
        }

        /// <summary>
        /// Открытие страницы с передачей информации о проведенном концерте
        /// </summary>
        public void Show(ConcertInfo concert)
        {
            _concert = concert;
            Open();
        }

        /// <summary>
        /// Влючаем нужную анимацию в зависимости от заполненности зала 
        /// </summary>
        private void FillDanceFloor()
        {
            var occupancyRatio = _concert.TicketsSold / (float) _concert.LocationCapacity;
            var locationOccupancyRatio = 1f / flexingStates.Length;
            var maxFlexingAnimationIndex = Mathf.FloorToInt(occupancyRatio / locationOccupancyRatio);
            maxFlexingAnimationIndex = Mathf.Clamp(maxFlexingAnimationIndex, 0, flexingStates.Length - 1);
            
            flexingGraphic.SetUpStatesOrder(flexingStates.Take(maxFlexingAnimationIndex + 1).ToArray());
        }

        protected override void AfterPageOpen()
        {
            var gender = PlayerManager.Data.Info.Gender;
            maleCharacter.gameObject.SetActive(gender == Gender.Male);
            femaleCharacter.gameObject.SetActive(gender == Gender.Female);
            
            flexingGraphic.color = FlexingGraphicStartColor;
            FillDanceFloor();
        }

        protected override void BeforePageClose()
        {
            flexingGraphic.AnimationState.SetEmptyAnimation(0, 0);
            _concert = null;
        }
    }
}