using System;
using Game.Settings;
using Models.Production;
using UnityEngine;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Production.Analyzers
{
    public class ConcertAnalyzer : Analyzer
    {
        public static void Analyze(ConcertInfo concert, GameSettings settings)
        {
            var album = IsPlayerCreator(concert.CreatorId)
                ? ProductionManager.GetAlbum(concert.AlbumId)
                : RappersAPI.Instance.GetAlbum(concert.CreatorId, concert.AlbumId);

            var concertQuality = CalculateWorkPointsFactor(
                concert.ManagementPoints,
                concert.MarketingPoints,
                settings.Concert.WorkPointsMax
            );
            concert.Quality = concertQuality;

            var fansAmount = GetFans(concert.CreatorId, settings.Player.BaseFans);

            var repeatsDebuff = 1f - album.ConcertAmounts / 10f;
            var costFactor    = 1.5f - 1f * concert.TicketCost / concert.MaxTicketCost;

            concert.TicketsSold = CalculateTicketSales(
                concert.CreatorId,
                fansAmount,
                concertQuality,
                album.Quality,
                costFactor,
                concert.LocationCapacity,
                repeatsDebuff,
                settings.Concert.MinTicketsSold
            );
        }

        /// <summary>
        ///     Вычисляет вклад рабочих очков в качество концерта
        /// </summary>
        private static float CalculateWorkPointsFactor(int manPoints, int marPoints, int maxWorkPoints)
        {
            var workPointsTotal = manPoints + marPoints;
            var qualityPercent  = 1f * workPointsTotal / maxWorkPoints;

            return Mathf.Min(qualityPercent, 1f);
        }

        /// <summary>
        ///     Вычисляет количество продаж на основе качества концерта, кол-ва фанатов и уровня хайпа
        /// </summary>
        private static int CalculateTicketSales(
            int   creatorId,
            int   fans,
            float quality,
            float albumQuality,
            float costFactor,
            int   capacity,
            float repeatsDebuff,
            int   minTicketsSold
        )
        {
            // Количество фанатов, ждущих трек, зависит от уровня хайпа
            var activeFansAmount = Convert.ToInt32(fans * (0.5f + GetHypeFactor(creatorId)));

            quality = Math.Max(1.0f, quality + Math.Min(0.5f, albumQuality));

            // Количество продаж билетов зависит от: качества концерта, прослушиваний альбома, цены билета и новизны альбома
            var salesFactor = quality * costFactor * repeatsDebuff;
            activeFansAmount = Convert.ToInt32(activeFansAmount * salesFactor);

            return Mathf.Clamp(activeFansAmount, minTicketsSold, capacity);
        }
    }
}