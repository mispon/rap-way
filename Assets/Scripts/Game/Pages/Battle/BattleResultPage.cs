using Core;
using Data;
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
            bool isWin = playerPoints != rapperPoints
                ? playerPoints > rapperPoints
                : Random.Range(0, 2) > 0;

            int fans = (int) settings.BattleFansChange.Evaluate(PlayerManager.Data.Fans) * (isWin ? +1 : -1);
            int hype = isWin ? settings.BattleWinnerHype : settings.BattleLoserHype;
            
            return new BattleResult
            {
                RapperInfo = rapper,
                FansIncome = fans,
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