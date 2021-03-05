using System;
using System.Collections.Generic;
using System.Linq;
using Data;
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
        
        public event Action<RapperInfo> onFeat = rapper => {};
        public event Action<RapperInfo> onBattle = rapper => {}; 
        
        private static PlayerData data => PlayerManager.Data;
        private static PlayerHistory playerHistory => data.History;
        
        /// <summary>
        /// Добавляет информацию о треке в историю игрока
        /// </summary>
        public static void AddTrack(TrackInfo info)
        {
            playerHistory.TrackList.AddProduction(info, Instance.onTrackAdd);
        }
        
        /// <summary>
        /// Возвращает экземпляр Трека по идентификатору
        /// </summary>
        public static TrackInfo GetTrack(int trackId)
            => playerHistory.TrackList.First(e => e.Id == trackId);

        /// <summary>
        /// Возвращает название трека по идентификатору
        /// </summary>
        public static string GetTrackName(int trackId)
        {
            var track = GetTrack(trackId);

            var featInfo = track.Feat != null ? $" feat. {track.Feat.Name}" : "";
            return $"{track.Name}{featInfo}";
        }

        /// <summary>
        /// Добавляет информацию об альбоме в историю игрока
        /// </summary>
        public static void AddAlbum(AlbumInfo info)
        {
            playerHistory.AlbumList.AddProduction(info, Instance.onAlbumAdd);
        }
        
        /// <summary>
        /// Возвращает экземпляр Альбома по идентификатору
        /// </summary>
        public static AlbumInfo GetAlbum(int albumId)
            => playerHistory.AlbumList.First(e => e.Id == albumId);

        /// <summary>
        /// Добавляет информацию о клипе в историю игрока
        /// </summary>
        public static void AddClip(ClipInfo info)
        {
            playerHistory.ClipList.AddProduction(info, Instance.onClipAdd);
        }
        
        /// <summary>
        /// Добавляет информацию о концерте в историю игрока
        /// </summary>
        public static void AddConcert(ConcertInfo info)
        {
            playerHistory.ConcertList.AddProduction(info, Instance.onConcertAdd);
        }

        /// <summary>
        /// Возвращает кол-во проведенных концертов по идентификатору альбома
        /// </summary>
        public static int SameConcertsCount(int albumId)
            => playerHistory.ConcertList.Count(c => c.AlbumId ==  albumId);

        /// <summary>
        /// Сохраняет информацию о фите
        /// </summary>
        public static void AddFeat(RapperInfo rapperInfo)
        {
            if (data.Feats.Contains(rapperInfo.Id))
                return;

            data.Feats.Add(rapperInfo.Id);
            Instance.onFeat.Invoke(rapperInfo);
        }

        /// <summary>
        /// Сохраняет информацию о батле
        /// </summary>
        public static void AddBattle(RapperInfo rapperInfo)
        {
            if (data.Battles.Contains(rapperInfo.Id))
                return;

            data.Battles.Add(rapperInfo.Id);
            Instance.onBattle.Invoke(rapperInfo);
        }
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