using Core.Context;
using Enums;
using Extensions;
using Game.Player;
using Game.Production;
using Game.Rappers.Desc;
using Game.Socials.Eagler;
using MessageBroker;
using MessageBroker.Messages.Production;
using Sirenix.OdinInspector;
using UI.Windows.GameScreen.Eagler;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Battle
{
    public class BattleResultPage : Page
    {
        [BoxGroup("Result"), SerializeField] private Text resultMessage;
        [BoxGroup("Result"), SerializeField] private Text fansIncome;
        [BoxGroup("Result"), SerializeField] private Text hypeIncome;
        [BoxGroup("Result"), SerializeField] private Text expIncome;
        
        [BoxGroup("Images"), SerializeField] private Image playerImage;
        [BoxGroup("Images"), SerializeField] private GameObject playerLoseMask;
        [BoxGroup("Images"), SerializeField] private Image rapperImage;
        [BoxGroup("Images"), SerializeField] private GameObject rapperLoseMask;
        
        [BoxGroup("Sprites"), SerializeField] private Sprite playerAvatarMale;
        [BoxGroup("Sprites"), SerializeField] private Sprite playerAvatarFemale;
        [BoxGroup("Sprites"), SerializeField] private Sprite customAvatar;
        
        [BoxGroup("Eagles"), SerializeField] private EagleCard[] eagleCards;
        
        private BattleResult _result;
        
        public override void Show(object ctx = null)
        {
            var rapper       = ctx.ValueByKey<RapperInfo>("rapper");
            var playerPoints = ctx.ValueByKey<int>("playerPoints");
            var rapperPoints = ctx.ValueByKey<int>("rapperPoints");
            
            _result = AnalyzeResult(rapper, playerPoints, rapperPoints);
            DisplayResult();
            
            base.Show(ctx);
        }
        
        /// <summary>
        /// Анализирует результаты батла 
        /// </summary>
        private BattleResult AnalyzeResult(RapperInfo rapper, int playerPoints, int rapperPoints)
        {
            bool isWin = playerPoints >= rapperPoints;
            
            int fansChange = (int) (PlayerManager.Data.Fans * 0.1f);
            int fansFuzz = (int) (fansChange * 0.1f);
            
            int hype = isWin ? settings.Battle.WinnerHype : settings.Battle.LoserHype;
            
            return new BattleResult
            {
                RapperInfo = rapper,
                FansIncome = Random.Range(fansChange - fansFuzz, fansChange + fansFuzz) * (isWin ? 1 : -1),
                HypeIncome = hype,
                IsWin = isWin
            };
        }

        /// <summary>
        /// Отображает результаты батла
        /// </summary>
        private void DisplayResult()
        {
            string playerName = PlayerManager.Data.Info.NickName.ToUpper();
            string rapperName = _result.RapperInfo.Name;

            resultMessage.text = _result.IsWin
                ? GetLocale("battle_result", playerName, rapperName)
                : GetLocale("battle_result", rapperName, playerName);

            string prefix = _result.FansIncome > 0 ? "+" : string.Empty;
            fansIncome.text = $"{prefix}{_result.FansIncome.GetDisplay()}";
            hypeIncome.text = $"+{_result.HypeIncome}";
            expIncome.text = $"+{settings.Battle.RewardExp}";

            playerImage.sprite = PlayerManager.Data.Info.Gender == Gender.Male
                ? playerAvatarMale
                : playerAvatarFemale;
            playerLoseMask.SetActive(!_result.IsWin);

            rapperImage.sprite = _result.RapperInfo.IsCustom
                ? customAvatar 
                : _result.RapperInfo.Avatar;
            rapperLoseMask.SetActive(_result.IsWin);

            DisplayEagles(0.5f);
        }

        private void DisplayEagles(float quality)
        {
            var eagles = EaglerManager.Instance.GenerateEagles(quality);
            for (var i = 0; i < eagles.Count; i++)
            {
                eagleCards[i].Initialize(i, eagles[i]);
            }
        }
        
        /// <summary>
        /// Сохраняет результаты батла
        /// </summary>
        private void SaveResult()
        {
            if (_result.IsWin)
            {
                ProductionManager.AddBattle(_result.RapperInfo);
            }
            
            MsgBroker.Instance.Publish(new ProductionRewardMessage
            {
                FansIncome = _result.FansIncome,
                HypeIncome = _result.HypeIncome,
                Exp = settings.Battle.RewardExp
            });
        }

        protected override void AfterHide()
        {
            SaveResult();
            _result = BattleResult.Empty;
        }
    }

    public struct BattleResult
    {
        public RapperInfo RapperInfo;
        public int FansIncome;
        public int HypeIncome;
        public bool IsWin;

        public static BattleResult Empty => new BattleResult
        {
            RapperInfo = null,
            FansIncome = 0,
            HypeIncome = 0,
            IsWin = false
        };
    }
}