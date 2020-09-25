using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Production> GetLastActions(int amount)
        {
            int size = TrackList.Count + AlbumList.Count + ClipList.Count + ConcertList.Count;
            var result = new List<Production>(size);
            
            result.AddRange(TrackList);
            result.AddRange(AlbumList);
            result.AddRange(ClipList);
            result.AddRange(ConcertList);

            return result.OrderByDescending(e => e.Timestamp).Take(amount).ToList();
        }
    }
}