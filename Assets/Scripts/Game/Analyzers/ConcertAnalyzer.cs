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

            float repeatsDebuff = album.ConcertAmounts / 10f;
            concertQuality -= repeatsDebuff;

            int fansAmount = PlayerManager.Data.Fans;
            concert.TicketsSold = CalculateTicketSales(concertQuality, fansAmount);
        }

        /// <summary>
        /// Вычисляет вклад рабочих очков в качество концерта
        /// </summary>
        private float CalculateWorkPointsFactor(int manPoints, int marPoints)
        {
            float workPointsPercent = (1f - settings.ConcertAlbumListensImpact) * (1f / settings.ConcertWorkPointsMax);
            float workPointsFactor = Mathf.Min(manPoints + marPoints, settings.ConcertWorkPointsMax) * workPointsPercent;
            return workPointsFactor;
        }

        /// <summary>
        /// Вычисляет количество продаж на основе качества концерта, кол-ва фанатов и уровня хайпа
        /// </summary>
        private int CalculateTicketSales(float concertQuality, int fansAmount)
        {
            float concertGrade = settings.ConcertGradeCurve.Evaluate(concertQuality);
            float hypeFactor = Mathf.Max(0.1f, PlayerManager.Data.Hype / 100f);

            int ticketSales = Convert.ToInt32(concertGrade * fansAmount * hypeFactor);

            return ticketSales;
        }
    }
}