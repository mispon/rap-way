﻿using Core;
using Core.Context;
using Enums;
using Core.Analytics;
using Game;
using Game.Labels.Desc;
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

namespace UI.Windows.GameScreen.Clip
{
    public class ClipWorkingPage : BaseWorkingPage
    {
        [Header("Work Points")]
        [SerializeField] private Text directorPoints;
        [SerializeField] private Text operatorPoints;

        [Header("Team")]
        [SerializeField] private WorkPoints playerWorkPoints;
        [SerializeField] private WorkPoints labelWorkPoints;
        [SerializeField] private WorkPoints directorWorkPoints;
        [SerializeField] private WorkPoints operatorWorkPoints;
        [SerializeField] private Image labelAvatar;
        [SerializeField] private GameObject labelFrozen;

        private ClipInfo _clip;
        private LabelInfo _label;

        public override void Show(object ctx = null)
        {
            StartWork(ctx);
            base.Show(ctx);
        }

        protected override void StartWork(object ctx)
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.CreateClipClick);

            _clip = ctx.Value<ClipInfo>();
            RefreshWorkAnims();
        }

        protected override void DoDayWork()
        {
            SoundManager.Instance.PlaySound(UIActionType.WorkPoint);
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        private void ShowResultPage()
        {
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.ProductionClipResult,
                Context = _clip
            });

            _clip = null;
        }

        protected override void FinishWork()
        {
            GameEventsManager.Instance.CallEvent(GameEventType.Clip, ShowResultPage);
        }

        protected override int GetDuration()
        {
            return settings.Clip.WorkDuration;
        }

        private void GenerateWorkPoints()
        {
            var directorPointsValue = Random.Range(1, _clip.DirectorSkill + 2);
            _clip.DirectorPoints += directorPointsValue;
            directorWorkPoints.Show(directorPointsValue);

            var operatorPointsValue = Random.Range(1, _clip.OperatorSkill + 2);
            _clip.OperatorPoints += operatorPointsValue;
            operatorWorkPoints.Show(operatorPointsValue);

            var playerPointsValue = Random.Range(1, PlayerAPI.Data.Stats.Charisma.Value + 2);
            playerWorkPoints.Show(playerPointsValue);

            if (Random.Range(0, 2) > 0)
                _clip.DirectorPoints += playerPointsValue;
            else
                _clip.OperatorPoints += playerPointsValue;

            var labelPoints = 0;
            if (_label is { IsFrozen: false })
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelWorkPoints.Show(labelPoints);
            }

            if (Random.Range(0, 2) > 0)
                _clip.DirectorPoints += labelPoints;
            else
                _clip.OperatorPoints += labelPoints;
        }

        private void DisplayWorkPoints()
        {
            directorPoints.text = _clip.DirectorPoints.ToString();
            operatorPoints.text = _clip.OperatorPoints.ToString();
        }

        protected override void BeforeShow(object ctx = null)
        {
            base.BeforeShow(ctx);

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
        }

        protected override void BeforeHide()
        {
            base.BeforeHide();

            directorPoints.text = "0";
            operatorPoints.text = "0";
        }
    }
}