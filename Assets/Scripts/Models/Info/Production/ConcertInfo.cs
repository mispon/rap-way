using System;
using Core;
using Localization;
using Utils.Extensions;

namespace Models.Info.Production
{
    /// <summary>
    /// Информация о проведенном концерте
    /// </summary>
    [Serializable]
    public class ConcertInfo: Production
    {
        public int AlbumId;

        public int LocationId;
        public string LocationName;
        public int LocationCapacity;
        
        public int MaxTicketCost;
        public int TicketCost;
        public int TicketsSold;

        public int ManagementPoints;
        public int MarketingPoints;
        
        public int Income => TicketsSold * TicketCost;

        public override string[] HistoryInfo => new[]
        {
            ProductionManager.GetAlbum(AlbumId).Name,
            LocationName,
            TicketsSold.GetDisplay(),
            Income.GetMoney()
        };
        
        public override string GetLog() {
            return $"{Timestamp}: {LocalizationManager.Instance.Get("log_concert")} \"{LocationName}\"";
        }
    }
}