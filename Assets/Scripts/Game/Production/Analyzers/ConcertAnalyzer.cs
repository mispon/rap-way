using System;
using Models.Production;
using UnityEngine;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Production.Analyzers
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
            var album = IsPlayerCreator(concert.CreatorId)
                ? ProductionManager.GetAlbum(concert.AlbumId)
                : RappersAPI.Instance.GetAlbum(concert.CreatorId, concert.AlbumId);

            float concertQuality = CalculateWorkPointsFactor(concert.ManagementPoints, concert.MarketingPoints);
            concert.Quality = concertQuality;

            int fansAmount = GetFans(concert.CreatorId);

            float repeatsDebuff = 1f - (album.ConcertAmounts / 10f);
            float costFactor = 1.5f - (1f * concert.TicketCost / concert.MaxTicketCost);

            concert.TicketsSold = CalculateTicketSales(
                concert.CreatorId,
                fansAmount,
                concertQuality,
                album.Quality,
                costFactor,
                concert.LocationCapacity,
                repeatsDebuff
            );
        }

        /// <summary>
        /// Вычисляет вклад рабочих очков в качество концерта
        /// </summary>
        private float CalculateWorkPointsFactor(int manPoints, int marPoints)
        {
            var workPointsTotal = manPoints + marPoints;
            var qualityPercent = (1f * workPointsTotal) / settings.Concert.WorkPointsMax;

            return Mathf.Min(qualityPercent, 1f);
        }

        /// <summary>
        /// Вычисляет количество продаж на основе качества концерта, кол-ва фанатов и уровня хайпа
        /// </summary>
        private int CalculateTicketSales(
            int creatorId,
            int fans,
            float quality,
            float albumQuality,
            float costFactor,
            int capacity,
            float repeatsDebuff
        )
        {
            // Количество фанатов, ждущих трек, зависит от уровня хайпа
            int activeFansAmount = Convert.ToInt32(fans * (0.5f + GetHypeFactor(creatorId)));

            quality = Math.Max(1.0f, quality + Math.Min(0.5f, albumQuality));

            // Количество продаж билетов зависит от: качества концерта, прослушиваний альбома, цены билета и новизны альбома
            float salesFactor = quality * costFactor * repeatsDebuff;
            activeFansAmount = Convert.ToInt32(activeFansAmount * salesFactor);

            var sold = Math.Min(activeFansAmount, capacity);
            return Math.Max(settings.Concert.MinTicketsSold, sold);
        }
    }
}