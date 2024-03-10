using Core;
using Core.Context;
using Enums;
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

namespace UI.Windows.GameScreen.Social
{
    public class SocialWorkingPage : BaseWorkingPage
    {
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text workPoints;
        [SerializeField] private WorkPoints playerWorkPoints;
        [SerializeField] private WorkPoints prManWorkPoints;
        [SerializeField] private Image prManAvatar;

        [Header("Данные")]
        [SerializeField] private ImagesBank imagesBank;
        
        private SocialInfo _social;
        private bool _hasPrMan;

        public override void Show(object ctx = null)
        {
            StartWork(ctx);
            base.Show(ctx);
        }

        protected override void StartWork(object ctx)
        {
            _social = ctx.Value<SocialInfo>();
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
            var playerPointsValue = Random.Range(1, PlayerAPI.Data.Stats.Charisma.Value + 2);
            playerWorkPoints.Show(playerPointsValue);

            var prManPointsValue = 0;
            if (_hasPrMan)
            {
                prManPointsValue = Random.Range(1, PlayerAPI.Data.Team.PrMan.Skill.Value + 2);
                prManWorkPoints.Show(prManPointsValue);
            }
            
            _social.WorkPoints += playerPointsValue + prManPointsValue;
        }

        private void DisplayWorkPoints()
        {
            workPoints.text = _social.WorkPoints.ToString();
        }

        protected override void FinishWork()
        {
            var windowType = _social.Type switch
            {
                SocialType.Charity   => WindowType.SocialsResult_Charity,
                SocialType.Trends    => WindowType.SocialsResult_Trends,
                SocialType.Eagler    => WindowType.SocialsResult_Eagler,
                SocialType.Ieyegram  => WindowType.SocialsResult_Ieyegram,
                SocialType.TackTack  => WindowType.SocialsResult_TackTack,
                SocialType.Telescope => WindowType.SocialsResult_Telescope,
                SocialType.Switch    => WindowType.SocialsResult_Switch,
                _ => throw new RapWayException($"Unknown socials type {_social.Type.ToString()}")
            };
            
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = windowType,
                Context = _social
            });
        }

        protected override int GetDuration()
        {
            return settings.Socials.WorkDuration;
        }

        protected override void BeforeShow(object ctx = null)
        {
            base.BeforeShow(ctx);
            
            _hasPrMan = TeamManager.IsAvailable(TeammateType.PrMan);
            prManAvatar.sprite = _hasPrMan ? imagesBank.PrManActive : imagesBank.PrManInactive;
        }

        protected override void BeforeHide()
        {
            base.BeforeHide();

            workPoints.text = "0";
            _social = null;
        }
    }
}