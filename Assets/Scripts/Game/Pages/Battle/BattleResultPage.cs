using Core;
using Data;
using Enums;
using Game.Pages.Eagler;
using Models.Game;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Battle
{
    /// <summary>
    /// Страница с результатами батла
    /// </summary>
    public class BattleResultPage : Page
    {
        [SerializeField] private Text resultMessage;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text hypeIncome;
        [SerializeField] private Text expIncome;
        [Header("Картинки")]
        [SerializeField] private Image playerImage;
        [SerializeField] private GameObject playerLoseMask;
        [SerializeField] private Image rapperImage;
        [SerializeField] private GameObject rapperLoseMask;
        [Header("Спрайты")]
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
            
            int hype = isWin ? settings.BattleWinnerHype : settings.BattleLoserHype;
            
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
            expIncome.text = $"+{settings.BattleRewardExp}";

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
            PlayerManager.Instance.AddFans(_result.FansIncome, settings.BattleRewardExp);
            PlayerManager.Instance.AddHype(_result.HypeIncome);

            if (_result.IsWin)
            {
                ProductionManager.AddBattle(_result.RapperInfo);
            }
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