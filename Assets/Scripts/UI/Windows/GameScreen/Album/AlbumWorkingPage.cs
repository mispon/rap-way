﻿using Core;
using Core.Context;
using Core.Analytics;
using Enums;
using Game;
using Game.Labels.Desc;
using Game.Player.State.Desc;
using Game.Player.Team;
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
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Album
{
    public class AlbumWorkingPage : BaseWorkingPage
    {
        [Header("Progress")]
        [SerializeField] private Text bitPoints;
        [SerializeField] private Text textPoints;

        [Header("Team")]
        [SerializeField] private WorkPoints playerBitWorkPoints;
        [SerializeField] private WorkPoints playerTextWorkPoints;
        [SerializeField] private WorkPoints bitmakerWorkPoints;
        [SerializeField] private WorkPoints textwritterWorkPoints;
        [SerializeField] private WorkPoints labelBitWorkPoints;
        [SerializeField] private WorkPoints labelTextWorkPoints;
        [SerializeField] private Image bitmakerAvatar;
        [SerializeField] private Image textwritterAvatar;
        [SerializeField] private Image labelAvatar;
        [SerializeField] private GameObject labelFrozen;

        [Header("Data"), SerializeField] private ImagesBank imagesBank;

        private bool _hasBitmaker;
        private bool _hasTextWriter;

        private AlbumInfo _album;
        private LabelInfo _label;

        public override void Show(object ctx = null)
        {
            StartWork(ctx);
            base.Show(ctx);
        }

        protected override void StartWork(object ctx)
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.CreateAlbumClick);

            _album = ctx.Value<AlbumInfo>();
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
                Type = WindowType.ProductionAlbumResult,
                Context = _album
            });

            _album = null;
        }

        protected override void FinishWork()
        {
            GameEventsManager.Instance.CallEvent(GameEventType.Album, ShowResultPage);
        }

        protected override int GetDuration()
        {
            return settings.Album.WorkDuration;
        }

        private void GenerateWorkPoints()
        {
            SoundManager.Instance.PlaySound(UIActionType.WorkPoint);

            _album.BitPoints += CreateBitPoints(PlayerAPI.Data);
            _album.TextPoints += CreateTextPoints(PlayerAPI.Data);
        }

        /// <summary>
        /// Creates work points in bit category
        /// </summary>
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
            if (_label is { IsFrozen: false })
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelBitWorkPoints.Show(labelPoints);
            }

            return playersBitPoints + bitmakerPoints + labelPoints;
        }

        /// <summary>
        /// Creates work points in text category
        /// </summary>
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
            if (_label is { IsFrozen: false })
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelTextWorkPoints.Show(labelPoints);
            }

            return playersTextPoints + textWriterPoints + labelPoints;
        }

        private void DisplayWorkPoints()
        {
            bitPoints.text = _album.BitPoints.ToString();
            textPoints.text = _album.TextPoints.ToString();
        }

        protected override void BeforeShow(object ctx = null)
        {
            base.BeforeShow(ctx);

            _hasBitmaker = TeamManager.IsAvailable(TeammateType.BitMaker);
            _hasTextWriter = TeamManager.IsAvailable(TeammateType.TextWriter);

            _label = !string.IsNullOrEmpty(PlayerAPI.Data.Label)
                ? LabelsAPI.Instance.Get(PlayerAPI.Data.Label)
                : null;

            if (_label != null)
            {
                labelAvatar.gameObject.SetActive(true);
                labelAvatar.sprite = _label.Logo;
                labelFrozen.SetActive(_label.IsFrozen);
            }
            else
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
        }
    }
}