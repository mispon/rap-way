﻿using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Models.Production;

namespace Game.Player.State.Desc
{
    [Serializable]
    public class ProductionHistory
    {
        public List<TrackInfo> TrackList;
        public List<AlbumInfo> AlbumList;
        public List<ClipInfo> ClipList;
        public List<ConcertInfo> ConcertList;

        public static ProductionHistory New => new ProductionHistory
        {
            TrackList = new List<TrackInfo>(),
            AlbumList = new List<AlbumInfo>(),
            ClipList = new List<ClipInfo>(),
            ConcertList = new List<ConcertInfo>()
        };

        public List<ProductionBase> GetLastActions(int amount)
        {
            int size = TrackList.Count + AlbumList.Count + ClipList.Count + ConcertList.Count;
            var result = new List<ProductionBase>(size);

            result.AddRange(TrackList);
            result.AddRange(AlbumList);
            result.AddRange(ClipList);
            result.AddRange(ConcertList);

            return result
                .OrderByDescending(e => e.Timestamp.StringToDate())
                .Take(amount)
                .ToList();
        }
    }
}