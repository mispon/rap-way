using System.Collections.Generic;
using Core;
using Core.Context;
using Enums;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Enums;
using UI.Windows.Pages;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Battle
{
    /// <summary>
    /// Рабочая страница батла
    /// </summary>
    public class BattleWorkingPage : BaseWorkingPage
    {
        [BoxGroup("Controls"), SerializeField] private Text playerName;
        [BoxGroup("Controls"), SerializeField] private Text rapperName;
        [BoxGroup("Controls"), SerializeField] private Image playerAvatar;
        [BoxGroup("Controls"), SerializeField] private Text playerPointsLabel;
        [BoxGroup("Controls"), SerializeField] private WorkPoints playerWorkPoints;
        [BoxGroup("Controls"), SerializeField] private Image rapperAvatar;
        [BoxGroup("Controls"), SerializeField] private Text rapperPointsLabel;
        [BoxGroup("Controls"), SerializeField] private WorkPoints rapperWorkPoints;
        [BoxGroup("Controls"), SerializeField] private Sprite customRapperAvatar;
        
        [BoxGroup("Work Points"), SerializeField] private int skillChance;
        [BoxGroup("Work Points"), SerializeField] private WorkPoints doubleTimePoint;
        [BoxGroup("Work Points"), SerializeField] private WorkPoints shoutOutPoint;
        [BoxGroup("Work Points"), SerializeField] private WorkPoints freestylePoint;
        [BoxGroup("Work Points"), SerializeField] private WorkPoints punchPoint;
        [BoxGroup("Work Points"), SerializeField] private WorkPoints flipPoint;
        
        [BoxGroup("Data"), SerializeField] private ImagesBank imagesBank;

        private RapperInfo _rapper;
        private int _playerPoints;
        private int _rapperPoints;

        public override void Show(object ctx = null)
        {
            StartWork(ctx);
            base.Show(ctx);
        }

        protected override void StartWork(object ctx)
        {
            _rapper = ctx.Value<RapperInfo>();
            RefreshWorkAnims();
        }
        
        protected override void DoDayWork()
        {
            SoundManager.Instance.PlaySound(UIActionType.WorkPoint);
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        protected override void FinishWork()
        {
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.BattleResult,
                Context = new Dictionary<string, object>
                {
                    ["rapper"]       = _rapper,
                    ["playerPoints"] = _playerPoints,
                    ["rapperPoints"] = _rapperPoints,
                }
            });
        }

        protected override int GetDuration()
        {
            return settings.Battle.WorkDuration;
        }

        private void GenerateWorkPoints()
        {
            int playerPoints = Random.Range(1, PlayerAPI.Data.Stats.Vocobulary.Value + 2);
            playerWorkPoints.Show(playerPoints);
            playerPoints += GenerateSkillsPoints();
            _playerPoints += playerPoints;

            int rapperPoints = Random.Range(1, _rapper.Vocobulary + 2);
            rapperWorkPoints.Show(rapperPoints);
            _rapperPoints += rapperPoints;
        }

        private int GenerateSkillsPoints()
        {
            int GeneratePoint(Skills skill, WorkPoints workPoints)
            {
                if (!PlayerAPI.Data.Skills.Contains(skill) || Random.Range(0, 100) > skillChance)
                    return 0;

                var value = Random.Range(1, 6);
                
                workPoints.Show(value);
                return value;
            }

            var result = GeneratePoint(Skills.DoubleTime, doubleTimePoint);
            result += GeneratePoint(Skills.ShoutOut, shoutOutPoint);
            result += GeneratePoint(Skills.Freestyle, freestylePoint);
            result += GeneratePoint(Skills.Punch, punchPoint);
            result += GeneratePoint(Skills.Flip, flipPoint);

            return result;
        }

        private void DisplayWorkPoints()
        {
            playerPointsLabel.text = _playerPoints.ToString();
            rapperPointsLabel.text = _rapperPoints.ToString();
        }

        private void DisplayAvailableSkills()
        {
            void DisplayIfAvailable(Skills skill, WorkPoints workPoint)
            {
                var icon = workPoint.transform.parent.GetComponent<Image>();
                icon.enabled = PlayerAPI.Data.Skills.Contains(skill);
            }

            DisplayIfAvailable(Skills.DoubleTime, doubleTimePoint);
            DisplayIfAvailable(Skills.ShoutOut, shoutOutPoint);
            DisplayIfAvailable(Skills.Freestyle, freestylePoint);
            DisplayIfAvailable(Skills.Punch, punchPoint);
            DisplayIfAvailable(Skills.Flip, flipPoint);
        }

        protected override void BeforeShow(object ctx = null)
        {
            base.BeforeShow(ctx);
            
            playerName.text = PlayerAPI.Data.Info.NickName.ToUpper();
            rapperName.text = _rapper.Name;

            _playerPoints = _rapperPoints = 0;
            playerPointsLabel.text = "0";
            rapperPointsLabel.text = "0";

            DisplayAvailableSkills();

            playerAvatar.sprite = PlayerAPI.Data.Info.Gender == Gender.Male
                ? imagesBank.MaleAvatar
                : imagesBank.FemaleAvatar;
            rapperAvatar.sprite = _rapper.IsCustom ? customRapperAvatar : _rapper.Avatar;
        }

        protected override void BeforeHide()
        {
            base.BeforeHide();
            _rapper = null;
        }
    }
}