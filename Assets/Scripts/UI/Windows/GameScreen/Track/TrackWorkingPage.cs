using Core;
using Core.Context;
using Enums;
using Firebase.Analytics;
using Game;
using Game.Labels.Desc;
using Game.Player;
using Game.Player.State.Desc;
using Game.Player.Team;
using MessageBroker;
using MessageBroker.Messages.UI;
using Models.Production;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Enums;
using UI.Windows.Pages;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Track
{
    /// <summary>
    /// Страница работы над треком
    /// </summary>
    public class TrackWorkingPage : BaseWorkingPage
    {
        [BoxGroup("Work Points"), SerializeField] private Text bitPoints;
        [BoxGroup("Work Points"), SerializeField] private Text textPoints;

        [BoxGroup("Team"), SerializeField] private WorkPoints playerBitWorkPoints;
        [BoxGroup("Team"), SerializeField] private WorkPoints playerTextWorkPoints;
        [BoxGroup("Team"), SerializeField] private WorkPoints bitmakerWorkPoints;
        [BoxGroup("Team"), SerializeField] private WorkPoints textwritterWorkPoints;
        [BoxGroup("Team"), SerializeField] private WorkPoints labelBitWorkPoints;
        [BoxGroup("Team"), SerializeField] private WorkPoints labelTextWorkPoints;
        [BoxGroup("Team"), SerializeField] private Image bitmakerAvatar;
        [BoxGroup("Team"), SerializeField] private Image textwritterAvatar;
        [BoxGroup("Team"), SerializeField] private Image labelAvatar;
        [BoxGroup("Team"), SerializeField] private GameObject labelFrozen;

        [BoxGroup("Images"), SerializeField] private ImagesBank imagesBank;

        private TrackInfo _track;
        private bool _hasBitmaker;
        private bool _hasTextWriter;
        private LabelInfo _label;

        public override void Show(object ctx = null)
        {
            StartWork(ctx);
            base.Show(ctx);
        }

        protected override void StartWork(object ctx)
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.CreateTrackClick);
            
            _track = ctx.Value<TrackInfo>();
            RefreshWorkAnims();
        }

        protected override void DoDayWork()
        {
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        private void ShowResultPage()
        {
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.ProductionTrackResult,
                Context = _track
            });
        }

        protected override void FinishWork()
        {
            GameEventsManager.Instance.CallEvent(GameEventType.Track, ShowResultPage);
        }

        protected override int GetDuration()
        {
            return settings.Track.WorkDuration;
        }

        private void GenerateWorkPoints()
        {
            SoundManager.Instance.PlaySound(UIActionType.WorkPoint);
            
            _track.BitPoints +=  CreateBitPoints(PlayerManager.Data);
            _track.TextPoints += CreateTextPoints(PlayerManager.Data);
        }

        private int CreateBitPoints(PlayerData data)
        {
            var playersBitPoints = Random.Range(1, data.Stats.Bitmaking.Value + 2);
            playerBitWorkPoints.Show(playersBitPoints);

            var bitmakerPoints = 0;
            if (_hasBitmaker)
            {
                bitmakerPoints = Random.Range(1, data.Team.BitMaker.Skill.Value + 2);
                bitmakerWorkPoints.Show(bitmakerPoints);
            }
            
            var labelPoints = 0;
            if (_label is {IsFrozen: false})
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelBitWorkPoints.Show(labelPoints);
            }

            return playersBitPoints + bitmakerPoints + labelPoints;
        }

        private int CreateTextPoints(PlayerData data)
        {
            var playersTextPoints = Random.Range(1, data.Stats.Vocobulary.Value + 2);
            playerTextWorkPoints.Show(playersTextPoints);

            var textWriterPoints = 0;
            if (_hasTextWriter)
            {
                textWriterPoints = Random.Range(1, data.Team.TextWriter.Skill.Value + 2);
                textwritterWorkPoints.Show(textWriterPoints);
            }
            
            var labelPoints = 0;
            if (_label is {IsFrozen: false})
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelTextWorkPoints.Show(labelPoints);
            }

            return playersTextPoints + textWriterPoints + labelPoints;
        }

        private void DisplayWorkPoints()
        {
            bitPoints.text = _track.BitPoints.ToString();
            textPoints.text = _track.TextPoints.ToString();
        }

        protected override void BeforeShow()
        {
            base.BeforeShow();
            
            _hasBitmaker = TeamManager.IsAvailable(TeammateType.BitMaker);
            _hasTextWriter = TeamManager.IsAvailable(TeammateType.TextWriter);
            
            if (!string.IsNullOrEmpty(PlayerManager.Data.Label))
            {
                _label = LabelsAPI.Instance.Get(PlayerManager.Data.Label);
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
            
            bitmakerAvatar.sprite = _hasBitmaker ? imagesBank.BitmakerActive : imagesBank.BitmakerInactive;
            textwritterAvatar.sprite = _hasTextWriter ? imagesBank.TextwritterActive : imagesBank.TextwritterInactive;
        }

        protected override void BeforeHide()
        {
            base.BeforeHide();
            
            bitPoints.text = textPoints.text = "0";
            _track = null;
        }
    }
}