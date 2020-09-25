using System;

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
        
        public int TicketCost;
        public int TicketsSold;

        public int ManagementPoints;
        public int MarketingPoints;
        
        public int Income => TicketsSold * TicketCost;

        public override string[] HistoryInfo => new[]
        {
            Name,
            LocationName,
            TicketCost.ToString(),
            Income.ToString()
        };
        
        public override string GetLog() {
            return $"{Timestamp}: Провёл концерт на площадке \"{LocationName}\"";
        }
    }
}