using System.Collections.Generic;
using Models.Production;

namespace Models.Player
{
    /// <summary>
    /// Информация о продуктах персонажа
    /// </summary>
    [System.Serializable]
    public class PlayerHistory
    {
        public List<TrackInfo> TrackList = new List<TrackInfo>();
        public List<AlbumInfo> AlbumList = new List<AlbumInfo>();
        public List<ClipInfo> ClipList = new List<ClipInfo>();
        public List<ConcertInfo> ConcertList = new List<ConcertInfo>();
    }
}