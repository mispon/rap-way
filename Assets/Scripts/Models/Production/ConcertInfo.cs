using Core;

namespace Models.Production
{
    /// <summary>
    /// Информация о проведенном концерте
    /// </summary>
    [System.Serializable]
    public class ConcertInfo: Production
    {
        public int AlbumId;
        public string Location;
        
        public int TicketsSold;
        public int OrganisationPoints;

        public int ManagerPoint;
        public int PrPoints;

        public override string ToString()
            => $"{DataManager.Instance.PlayerData.Info.NickName} - {Name} (Live: {Location})";
    }
}