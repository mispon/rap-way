using System.Collections.Generic;
using Core;
using Core.Context;
using Enums;
using Game.Player.Team;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Enums;
using UI.Windows.Pages;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Rappers
{
    public class RapperWorkingPage : BaseWorkingPage
    {
        [BoxGroup("Work Points"), SerializeField] private Text managementPointsLabel;
        [BoxGroup("Work Points"), SerializeField] private Text rapperPointsLabel;

        [BoxGroup("Characters"), SerializeField] private Image managerAvatar;
        [BoxGroup("Characters"), SerializeField] private Image rapperAvatar;
        [BoxGroup("Characters"), SerializeField] private WorkPoints playerWorkPoints;
        [BoxGroup("Characters"), SerializeField] private WorkPoints managerWorkPoints;
        [BoxGroup("Characters"), SerializeField] private WorkPoints rapperWorkPoints;
        [BoxGroup("Characters"), SerializeField] private Sprite customRapperAvatar;
        [BoxGroup("Characters"), SerializeField] private Text playerHypeBonus;

        [BoxGroup("Images"), SerializeField] private ImagesBank imagesBank;

        private RapperInfo _rapper;
        private ConversationType _convType;
        private int _playerPoints;
        private int _rapperPoints;
        private bool _hasManager;


        public override void Show(object ctx = null)
        {
            StartWork(ctx);
            base.Show(ctx);
        }

        protected override void StartWork(object ctx)
        {
            _rapper   = ctx.ValueByKey<RapperInfo>("rapper"); 
            _convType = ctx.ValueByKey<ConversationType>("conv_type");
            
            RefreshWorkAnims();
        }

        protected override void DoDayWork()
        {
            SoundManager.Instance.PlaySound(UIActionType.WorkPoint);
            GeneratePlayerWorkPoints();
            GenerateRapperWorkPoints();
        }

        protected override void FinishWork()
        {
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.RapperConversationsResult,
                Context = new Dictionary<string, object>
                {
                    ["rapper"]        = _rapper,
                    ["player_points"] = _playerPoints,
                    ["rapper_points"] = _rapperPoints,
                    ["conv_type"]     = _convType
                }
            });
        }

        protected override int GetDuration()
        {
            return settings.Rappers.ConversationDuration;
        }

        private void GeneratePlayerWorkPoints()
        {
            int playerPoints = Random.Range(1, PlayerAPI.Data.Stats.Management.Value + 1);
            playerWorkPoints.Show(playerPoints);

            int managerPoints = 0;
            if (_hasManager)
            {
                managerPoints = Random.Range(1, PlayerAPI.Data.Team.Manager.Skill.Value + 1);
                managerWorkPoints.Show(managerPoints);
            }

            _playerPoints += playerPoints + managerPoints;
            managementPointsLabel.text = $"{_playerPoints}";
        }

        private void GenerateRapperWorkPoints()
        {
            int rapperPoints =  Random.Range(1, _rapper.Management + 1);
            rapperWorkPoints.Show(rapperPoints);
            _rapperPoints += rapperPoints;
            rapperPointsLabel.text = $"{_rapperPoints}";
        }

        protected override void BeforeShow(object ctx = null)
        {
            base.BeforeShow(ctx);
            
            _hasManager = TeamManager.IsAvailable(TeammateType.Manager);
            managerAvatar.sprite = _hasManager ? imagesBank.ProducerActive : imagesBank.ProducerInactive;
            
            rapperAvatar.sprite = _rapper.IsCustom ? customRapperAvatar : _rapper.Avatar;
            managementPointsLabel.text = "0";
            rapperPointsLabel.text = "0";
            playerHypeBonus.text = $"+{PlayerAPI.Data.Hype / 5}";
            _playerPoints = 0;
            _rapperPoints = 0;
        }

        protected override void BeforeHide()
        {
            base.BeforeHide();
            _rapper = null;
        }
    }
}