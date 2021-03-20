using System;
using Core;
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
            var album = ProductionManager.GetAlbum(concert.AlbumId);

            float listenImpact = settings.ConcertAlbumListensImpact;
            float albumListenFactor = Mathf.Min(listenImpact * GetListenRatio(album.ListenAmount), listenImpact);
            float concertQuality = albumListenFactor + CalculateWorkPointsFactor(concert.ManagementPoints, concert.MarketingPoints);
            concert.Quality = concertQuality;

            float repeatsDebuff = album.ConcertAmounts / 10f;
            float costFactor = 1 - (1f * concert.TicketCost / concert.MaxTicketCost);
            concertQuality = Mathf.Clamp(concertQuality - repeatsDebuff, 0.1f, 1f);

            concert.TicketsSold = CalculateTicketSales(concertQuality, costFactor, concert.LocationCapacity);
        }

        /// <summary>
        /// Вычисляет вклад рабочих очков в качество концерта
        /// </summary>
        private float CalculateWorkPointsFactor(int manPoints, int marPoints)
        {
            float workPointsImpact = 1f - settings.ConcertAlbumListensImpact;
            float workPointsRatio = 1f * (manPoints + marPoints) / settings.ConcertWorkPointsMax;

            float workPointsFactor = workPointsImpact * Mathf.Min(workPointsRatio, 1f);

            return workPointsFactor;
        }

        /// <summary>
        /// Вычисляет количество продаж на основе качества концерта, кол-ва фанатов и уровня хайпа
        /// </summary>
        private int CalculateTicketSales(float concertQuality, float costFactor, int capacity)
        {
            float grade = settings.ConcertGradeCurve.Evaluate(concertQuality);
            float hypeFactor = PlayerManager.Data.Hype / 100f;

            float totalFactor = Mathf.Clamp(hypeFactor + costFactor, 0.1f, 1.0f);
            int ticketSales = Convert.ToInt32(capacity * grade * totalFactor);

            return ticketSales;
        }
    }
}