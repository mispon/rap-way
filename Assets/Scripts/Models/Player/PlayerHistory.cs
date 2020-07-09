using System;
using System.Collections.Generic;
using Models.Info.Production;

namespace Models.Player
{
    /// <summary>
    /// Информация о продуктах персонажа
    /// </summary>
    [Serializable]
    public class PlayerHistory
    {
        public List<TrackInfo> TrackList;
        public List<AlbumInfo> AlbumList;
        public List<ClipInfo> ClipList;
        public List<ConcertInfo> ConcertList;
        
        public static PlayerHistory New => new PlayerHistory
        {
            TrackList = new List<TrackInfo>(),
            AlbumList = new List<AlbumInfo>(),
            ClipList = new List<ClipInfo>(),
            ConcertList = new List<ConcertInfo>()
        };
    }
}