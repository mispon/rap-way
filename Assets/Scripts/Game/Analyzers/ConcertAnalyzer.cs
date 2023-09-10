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
            
            float concertQuality = CalculateWorkPointsFactor(concert.ManagementPoints, concert.MarketingPoints);
            concert.Quality = concertQuality;
            
            float albumListenFactor = GetListenRatio(album.ListenAmount);
            float repeatsDebuff = 1f - (album.ConcertAmounts / 10f);
            float costFactor = 1.5f - (1f * concert.TicketCost / concert.MaxTicketCost);
            
            concert.TicketsSold = CalculateTicketSales(
                concertQuality, 
                costFactor, 
                concert.LocationCapacity,
                albumListenFactor,
                repeatsDebuff
            );
        }

        /// <summary>
        /// Вычисляет вклад рабочих очков в качество концерта
        /// </summary>
        private float CalculateWorkPointsFactor(int manPoints, int marPoints)
        {
            var workPointsTotal = manPoints + marPoints;
            var qualityPercent = (1f * workPointsTotal) / settings.ConcertWorkPointsMax;

            return Mathf.Min(qualityPercent, 1f);
        }

        /// <summary>
        /// Вычисляет количество продаж на основе качества концерта, кол-ва фанатов и уровня хайпа
        /// </summary>
        private int CalculateTicketSales(
            float concertQuality, 
            float costFactor, 
            int capacity,
            float albumListenFactor,
            float repeatsDebuff
        ) {
            // Количество фанатов, ждущих трек, зависит от уровня хайпа
            int activeFansAmount = (int) (GetFans() * GetHypeFactor());

            // Количество продаж билетов зависит от: качества концерта, прослушиваний альбома, цены билета и новизны альбома
            float salesFactor = concertQuality * albumListenFactor * costFactor * repeatsDebuff;
            activeFansAmount = (int) (activeFansAmount * salesFactor);
            
            var sold = Math.Min(activeFansAmount, capacity);
            return Math.Max(0, sold);
        }
    }
}