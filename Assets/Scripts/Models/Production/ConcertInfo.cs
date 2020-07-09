using System;

namespace Models.Production
{
    /// <summary>
    /// Информация о проведенном концерте
    /// </summary>
    [Serializable]
    public class ConcertInfo: Production
    {
        public int AlbumId;
        
        public string LocationName;
        public int LocationCapacity;
        
        public int TicketCost;
        public int TicketsSold;

        public int ManagementPoints;
        public int MarketingPoints;
        
        public int Income => TicketsSold * TicketCost;
    }
}