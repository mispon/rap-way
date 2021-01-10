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
        [Space]
        [SerializeField] private int rewardExp;
        [SerializeField] private AnimationCurve fansChange;
        [SerializeField] private int winnerHype;
        [SerializeField] private int loserHype;

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

            int fans = (int) fansChange.Evaluate(PlayerManager.Data.Fans) * (isWin ? +1 : -1);
            int hype = isWin ? winnerHype : loserHype;
            
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
            var rapperName = _result.RapperInfo.Name;
            resultMessage.text = _result.IsWin
                ? $"<color=#CE93F1>{PlayerManager.Data.Info.NickName}</color> победил в батле с <color=#F6C326>{rapperName}</color>!"
                : $"<color=#F6C326>{rapperName}</color> победил в батле с <color=#CE93F1>{PlayerManager.Data.Info.NickName}</color>!";

            hypeIncome.text = $"{_result.HypeIncome}";
            fansIncome.text = $"{_result.FansIncome.GetDisplay()}";
        }

        /// <summary>
        /// Сохраняет результаты батла
        /// </summary>
        private void SaveResult()
        {
            PlayerManager.Instance.AddFans(_result.FansIncome, rewardExp);
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