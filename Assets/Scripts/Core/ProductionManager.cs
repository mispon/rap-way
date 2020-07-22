using System;
using System.Collections.Generic;
using Game;
using Models.Info.Production;
using Models.Player;
using Utils;

namespace Core
{
    /// <summary>
    /// Менеджер добавления новых Production в историю игрока.
    /// </summary>
    public class ProductionManager: Singleton<ProductionManager>
    {
        public event Action<TrackInfo> onTrackAdd = info => { }; 
        public event Action<AlbumInfo> onAlbumAdd = info => { }; 
        public event Action<ClipInfo> onClipAdd = info => { }; 
        public event Action<ConcertInfo> onConcertAdd = info => { };
        
        private static PlayerHistory playerHistory => PlayerManager.Data.History;

        public static void AddTrack(TrackInfo info)
        {
            playerHistory.TrackList.AddProduction(info, Instance.onTrackAdd);
        }
        
        public static void AddAlbum(AlbumInfo info)
        {
            playerHistory.AlbumList.AddProduction(info, Instance.onAlbumAdd);
        }
        
        public static void AddClip(ClipInfo info)
        {
            playerHistory.ClipList.AddProduction(info, Instance.onClipAdd);
        }
        
        public static void AddConcert(ConcertInfo info)
        {
            playerHistory.ConcertList.AddProduction(info, Instance.onConcertAdd);
        }
    }

    public static class Extension
    {
        /// <summary>
        /// Добавляет Production в список истории игрока и вызывает связанное событие
        /// </summary>
        public static void AddProduction<T>(this List<T> productionList, T info, Action<T> newProductionEvent) where T : Production
        {
            productionList.Add(info);
            newProductionEvent.Invoke(info);
        }
    }
}