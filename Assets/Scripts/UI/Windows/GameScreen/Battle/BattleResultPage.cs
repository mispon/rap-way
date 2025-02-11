using Core;
using Core.Context;
using Extensions;
using Game.Production;
using Game.Rappers.Desc;
using Game.SocialNetworks.Eagler;
using MessageBroker;
using MessageBroker.Messages.Production;
using MessageBroker.Messages.SocialNetworks;
using ScriptableObjects;
using UI.Windows.GameScreen.SocialNetworks.Eagler;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Battle
{
    public class BattleResultPage : Page
    {
        [Header("Result")]
        [SerializeField] private Text resultMessage;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text hypeIncome;
        [SerializeField] private Text expIncome;

        [Header("Images")]
        [SerializeField] private Image playerImage;
        [SerializeField] private GameObject playerLoseMask;
        [SerializeField] private Image      rapperImage;
        [SerializeField] private GameObject rapperLoseMask;

        [Header("Sprites")]
        [SerializeField] private Sprite customAvatar;

        [Header("Eagles")]
        [SerializeField] private EaglerCard[] eagleCards;

        [Header("Images")]
        [SerializeField] private ImagesBank imagesBank;

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

        private BattleResult AnalyzeResult(RapperInfo rapper, int playerPoints, int rapperPoints)
        {
            var isWin = playerPoints >= rapperPoints;

            var fansChange = (int) (PlayerAPI.Data.Fans * 0.1f);
            var fansFuzz   = (int) (fansChange * 0.1f);

            var hype = isWin ? settings.Battle.WinnerHype : settings.Battle.LoserHype;

            return new BattleResult
            {
                RapperInfo = rapper,
                FansIncome = Random.Range(fansChange - fansFuzz, fansChange + fansFuzz) * (isWin ? 1 : -1),
                HypeIncome = hype,
                IsWin      = isWin
            };
        }

        private void DisplayResult()
        {
            var playerName = PlayerAPI.Data.Info.NickName.ToUpper();
            var rapperName = _result.RapperInfo.Name;

            resultMessage.text = _result.IsWin
                ? GetLocale("battle_result", playerName, rapperName)
                : GetLocale("battle_result", rapperName, playerName);

            var prefix = _result.FansIncome > 0 ? "+" : string.Empty;
            fansIncome.text = $"{prefix}{_result.FansIncome.GetDisplay()}";
            hypeIncome.text = $"+{_result.HypeIncome}";
            expIncome.text  = $"+{settings.Battle.RewardExp}";

            playerImage.sprite = SpritesManager.Instance.GetPortrait(playerName);
            playerLoseMask.SetActive(!_result.IsWin);

            rapperImage.sprite = _result.RapperInfo.IsCustom
                ? customAvatar
                : _result.RapperInfo.Avatar;
            rapperLoseMask.SetActive(_result.IsWin);

            DisplayEagles(0.5f);
        }

        private void DisplayEagles(float quality)
        {
            var nickname = PlayerAPI.Data.Info.NickName;
            var fans     = PlayerAPI.Data.Fans;

            var eagles = EaglerManager.Instance.GenerateEagles(3, nickname, fans, quality);
            for (var i = 0; i < eagles.Count; i++)
            {
                eagleCards[i].Initialize(i, eagles[i]);
            }
        }

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
                Exp        = settings.Battle.RewardExp
            });
        }

        protected override void AfterHide()
        {
            SaveResult();

            var playerName = PlayerAPI.Data.Info.NickName.ToUpper();
            var rapperName = _result.RapperInfo.Name;

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_battle_finished",
                TextArgs = _result.IsWin
                    ? new[] {playerName, rapperName}
                    : new[] {rapperName, playerName},
                Sprite     = imagesBank.NewsBattle,
                Popularity = PlayerAPI.Data.Fans
            });

            _result = BattleResult.Empty;
        }
    }

    public struct BattleResult
    {
        public RapperInfo RapperInfo;
        public int        FansIncome;
        public int        HypeIncome;
        public bool       IsWin;

        public static BattleResult Empty => new()
        {
            RapperInfo = null,
            FansIncome = 0,
            HypeIncome = 0,
            IsWin      = false
        };
    }
}