using Enums;
using Extensions;
using Game.Player;
using Game.Production;
using Game.Rappers.Desc;
using Game.Socials.Eagler;
using MessageBroker;
using MessageBroker.Messages.Production;
using UI.Windows.GameScreen;
using UI.Windows.Pages.Eagler;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Battle
{
    public class BattleResultPage : Page
    {
        [SerializeField] private Text resultMessage;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text hypeIncome;
        [SerializeField] private Text expIncome;
        [Header("Images")]
        [SerializeField] private Image playerImage;
        [SerializeField] private GameObject playerLoseMask;
        [SerializeField] private Image rapperImage;
        [SerializeField] private GameObject rapperLoseMask;
        [Header("Sprites")]
        [SerializeField] private Sprite playerAvatarMale;
        [SerializeField] private Sprite playerAvatarFemale;
        [SerializeField] private Sprite customAvatar;
        [Space]
        [SerializeField] private EagleCard[] eagleCards;
        
        private BattleResult _result;
        
        /// <summary>
        /// Открывает страницу результатов
        /// </summary>
        public void Show(RapperInfo rapper, int playerPoints, int rapperPoints)
        {
            _result = AnalyzeResult(rapper, playerPoints, rapperPoints);
            DisplayResult();
            Open();
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

        protected override void AfterPageClose()
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