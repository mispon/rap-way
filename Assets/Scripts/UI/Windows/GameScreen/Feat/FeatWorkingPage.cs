using Core;
using Core.Context;
using MessageBroker;
using MessageBroker.Messages.UI;
using Models.Production;
using ScriptableObjects;
using UI.Enums;
using UI.Windows.Pages;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Feat
{
    public class FeatWorkingPage : BaseWorkingPage
    {
        [Header("Work Points")]
        [SerializeField] private Text bitPoints;
        [SerializeField] private Text textPoints;

        [Header("Team")]
        [SerializeField] private WorkPoints playerBitWorkPoints;
        [SerializeField] private WorkPoints playerTextWorkPoints;
        [SerializeField] private WorkPoints rapperBitWorkPoints;
        [SerializeField] private WorkPoints rapperTextWorkPoints;
        [SerializeField] private Image rapperAvatar;
        [SerializeField] private Sprite customRapperAvatar;

        private TrackInfo _track;

        public override void Show(object ctx = null)
        {
            StartWork(ctx);
            base.Show(ctx);
        }

        protected override void StartWork(object ctx)
        {
            _track = ctx.Value<TrackInfo>();
            RefreshWorkAnims();
        }

        protected override void DoDayWork()
        {
            SoundManager.Instance.PlaySound(UIActionType.WorkPoint);
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        private void GenerateWorkPoints()
        {
            var stats = PlayerAPI.Data.Stats;
            
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

        private void DisplayWorkPoints()
        {
            bitPoints.text = _track.BitPoints.ToString();
            textPoints.text = _track.TextPoints.ToString();
        }

        protected override void FinishWork()
        {
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.ProductionFeatResult,
                Context = _track
            });
        }

        protected override int GetDuration()
        {
            return settings.Track.FeatWorkDuration;
        }
        
        protected override void BeforeShow(object ctx = null)
        {
            base.BeforeShow(ctx);
            
            bitPoints.text = textPoints.text = "0";
            rapperAvatar.sprite = _track.Feat.IsCustom ? customRapperAvatar : _track.Feat.Avatar;
        }

        protected override void BeforeHide()
        {
            base.BeforeHide();
            _track = null;
        }
    }
}