using System;
using Game;

namespace Models.Production
{
    /// <summary>
    /// Информация о проведенном концерте
    /// </summary>
    [Serializable]
    public class ConcertInfo: Production
    {
        public int AlbumId;
        public string Location;
        
        public int TicketsSold;
        public int OrganisationPoints;

        public int ManagerPoint;
        public int PrPoints;

        public override string ToString()
            => $"{GameManager.Instance.PlayerData.Info.NickName} - {Name} (Live: {Location})";
    }
}