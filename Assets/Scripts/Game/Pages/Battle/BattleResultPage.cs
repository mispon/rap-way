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
        [Space]
        [SerializeField] private int rewardExp;
        [SerializeField] private AnimationCurve fansChange;
        [SerializeField] private int winnerHype;
        [SerializeField] private int loserHype;

        /// <summary>
        /// Открывает страницу результатов
        /// </summary>
        public void Show(RapperInfo rapper, int playerPoints, int rapperPoints)
        {
            var result = AnalyzeResult(rapper.Id, playerPoints, rapperPoints);
            
            DisplayResult(rapper.Name, in result);
            SaveResult(in result);
            
            Open();
        }
        
        /// <summary>
        /// Анализирует результаты батла 
        /// </summary>
        private BattleResult AnalyzeResult(int rapperId, int playerPoints, int rapperPoints)
        {
            bool isWin = playerPoints != rapperPoints
                ? playerPoints > rapperPoints
                : Random.Range(0, 2) > 0;

            int fans = (int) fansChange.Evaluate(PlayerManager.Data.Fans) * (isWin ? +1 : -1);
            int hype = isWin ? winnerHype : loserHype;
            
            return new BattleResult
            {
                RapperId = rapperId,
                FansIncome = fans,
                HypeIncome = hype,
                IsWin = isWin
            };
        }

        /// <summary>
        /// Отображает результаты батла
        /// </summary>
        private void DisplayResult(string rapperName, in BattleResult result)
        {
            resultMessage.text = result.IsWin
                ? $"{PlayerManager.Data.Info.NickName} победил в батле с {rapperName}!"
                : $"{rapperName} победил в батле с {PlayerManager.Data.Info.NickName}!";
            
            fansIncome.text = $"ФАНАТЫ: {result.FansIncome.GetDisplay()}";
            hypeIncome.text = $"ХАЙП: {result.HypeIncome}";
        }

        /// <summary>
        /// Сохраняет результаты батла
        /// </summary>
        private void SaveResult(in BattleResult result)
        {
            PlayerManager.Instance.AddFans(result.FansIncome, rewardExp);
            PlayerManager.Instance.AddHype(result.HypeIncome);
            
            if (result.IsWin)
                PlayerManager.Instance.SaveBattle(result.RapperId);
        }
    }

    public struct BattleResult
    {
        public int RapperId;
        public int FansIncome;
        public int HypeIncome;
        public bool IsWin;
    }
}