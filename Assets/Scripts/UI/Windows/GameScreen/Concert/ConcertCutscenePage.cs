using System.Linq;
using Enums;
using Extensions;
using Models.Production;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Concert
{
    public class ConcertCutscenePage : Page
    {
        private static readonly Color FlexingGraphicStartColor = new(1, 1, 1, 0);

        [Header("Персонаж")]
        [SerializeField] private Image maleCharacter;
        [SerializeField] private SkeletonGraphic femaleCharacter;

        [Header("Анимации флекса")]
        [SerializeField] private SkeletonGraphic flexingGraphic;
        [SerializeField] private Animation flexingAnim;

        [SerializeField, SpineAnimation(dataField = "flexingGraphic")]
        private string[] flexingStates;

        [Header("Пропуск катсцены")]
        [SerializeField] private Button skipButton;

        private ConcertInfo _concert;

        private void Start()
        {
            skipButton.onClick.AddListener(base.Hide);
        }

        public void Show(ConcertInfo concert)
        {
            _concert = concert;
            base.Show();
        }

        private void FillDanceFloor()
        {
            var occupancyRatio = _concert.TicketsSold / (float)_concert.LocationCapacity;
            var locationOccupancyRatio = 1f / flexingStates.Length;
            var maxFlexingAnimationIndex = Mathf.FloorToInt(occupancyRatio / locationOccupancyRatio);
            maxFlexingAnimationIndex = Mathf.Clamp(maxFlexingAnimationIndex, 0, flexingStates.Length - 1);

            flexingGraphic.SetUpStatesOrder(flexingStates.Take(maxFlexingAnimationIndex + 1).ToArray());
            flexingAnim.Play();
        }

        protected override void AfterShow(object ctx = null)
        {
            var gender = PlayerAPI.Data.Info.Gender;
            maleCharacter.gameObject.SetActive(gender == Gender.Male);
            femaleCharacter.gameObject.SetActive(gender == Gender.Female);

            flexingGraphic.color = FlexingGraphicStartColor;
            FillDanceFloor();
        }

        protected override void BeforeHide()
        {
            flexingGraphic.AnimationState.SetEmptyAnimation(0, 0);
            _concert = null;
        }
    }
}