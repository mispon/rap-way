using Models.Info.Production;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор концерта
    /// </summary>
    public class ConcertAnalyzer : Analyzer<ConcertInfo>
    {
        /// <summary>
        /// Анализирует успешность концерта
        /// </summary>
        public override void Analyze(ConcertInfo concert)
        {
            // todo: analyze concert

            concert.TicketsSold = Random.Range(1, 1000);

            concert.FansIncome = Random.Range(1, 100);
            concert.MoneyIncome = Random.Range(1, 100);
        }
    }
}