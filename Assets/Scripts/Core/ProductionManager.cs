using System;
using System.Collections.Generic;
using System.Linq;
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

        #region TRACK
        
        public static void AddTrack(TrackInfo info)
        {
            playerHistory.TrackList.AddProduction(info, Instance.onTrackAdd);
        }
        
        /// <summary>
        /// Возвращает экземпляр Трека по идентификатору
        /// </summary>
        public static TrackInfo GetTrack(int trackId)
            => PlayerManager.Data.History.TrackList.First(e => e.Id == trackId);
        
        /// <summary>
        /// Возвращает название трека по идентификатору
        /// </summary>
        public static string GetTrackName(int trackId)
        {
            var track = GetTrack(trackId);

            var featInfo = track.Feat != null ? $" feat. {track.Feat.Name}" : "";
            return $"{track.Name}{featInfo}";
        }
        
        #endregion

        #region ALBUM

        public static void AddAlbum(AlbumInfo info)
        {
            playerHistory.AlbumList.AddProduction(info, Instance.onAlbumAdd);
        }
        
        /// <summary>
        /// Возвращает экземпляр Альбома по идентификатору
        /// </summary>
        public static AlbumInfo GetAlbum(int albumId)
            => PlayerManager.Data.History.AlbumList.First(e => e.Id == albumId);

        #endregion
        
        
        public static void AddClip(ClipInfo info)
        {
            playerHistory.ClipList.AddProduction(info, Instance.onClipAdd);
        }

        #region CONCERT

        public static void AddConcert(ConcertInfo info)
        {
            playerHistory.ConcertList.AddProduction(info, Instance.onConcertAdd);
        }

        /// <summary>
        /// Возвращает кол-во проведенных концертов по идентификатору альбома
        /// </summary>
        public static int SameConcertsCount(int albumId)
            => PlayerManager.Data.History.ConcertList.Count(c => c.AlbumId ==  albumId);

        #endregion
    }

    public static partial class Extension
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