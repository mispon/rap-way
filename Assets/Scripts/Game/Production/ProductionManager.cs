using Core;
using Core.Analytics;
using Enums;
using Game.Player.State.Desc;
using Game.Rappers.Desc;
using System;
using System.Collections.Generic;
using System.Linq;
using Models.Production;

namespace Game.Production
{
    public class ProductionManager : Singleton<ProductionManager>
    {
        public event Action<TrackInfo> onTrackAdd = info => { };
        public event Action<AlbumInfo> onAlbumAdd = info => { };
        public event Action<ClipInfo> onClipAdd = info => { };
        public event Action<ConcertInfo> onConcertAdd = info => { };

        public event Action<RapperInfo> onFeat = rapper => { };
        public event Action<RapperInfo> onBattle = rapper => { };

        private static PlayerData data => GameManager.Instance.PlayerData;
        private static PlayerHistory playerHistory => data.History;

        public static void AddTrack(TrackInfo info)
        {
            switch (playerHistory.TrackList.Count)
            {
                case 0:
                    AnalyticsManager.LogEvent(FirebaseGameEvents.PlayerCreateFirstTrack);
                    break;
                case 1:
                    AnalyticsManager.LogEvent(FirebaseGameEvents.PlayerCreateSecondTrack);
                    break;
            }

            playerHistory.TrackList.AddProduction(info, Instance.onTrackAdd);
        }

        public static TrackInfo GetTrack(int trackId)
        {
            return playerHistory.TrackList.First(e => e.Id == trackId);
        }

        public static string GetTrackName(int trackId)
        {
            var track = GetTrack(trackId);

            var featInfo = track.Feat != null && !string.IsNullOrWhiteSpace(track.Feat.Name)
                ? $" feat. {track.Feat.Name}"
                : string.Empty;

            return $"{track.Name}{featInfo}";
        }

        public static void AddAlbum(AlbumInfo info)
        {
            playerHistory.AlbumList.AddProduction(info, Instance.onAlbumAdd);
        }

        public static AlbumInfo GetAlbum(int albumId)
        {
            return playerHistory.AlbumList.First(e => e.Id == albumId);
        }

        public static void AddClip(ClipInfo info)
        {
            playerHistory.ClipList.AddProduction(info, Instance.onClipAdd);
        }

        public static void AddConcert(ConcertInfo info)
        {
            playerHistory.ConcertList.AddProduction(info, Instance.onConcertAdd);
        }

        public static void AddFeat(RapperInfo rapperInfo)
        {
            if (data.Feats.Contains(rapperInfo.Id))
                return;

            data.Feats.Add(rapperInfo.Id);
            Instance.onFeat.Invoke(rapperInfo);
        }

        public static void AddBattle(RapperInfo rapperInfo)
        {
            if (data.Battles.Contains(rapperInfo.Id))
                return;

            data.Battles.Add(rapperInfo.Id);
            Instance.onBattle.Invoke(rapperInfo);
        }

        public static int GetNextProductionId<T>() where T : ProductionBase
        {
            var history = data.History;
            var id = 0;

            if (typeof(T) == typeof(TrackInfo))
                id = history.TrackList.Any() ? history.TrackList.Max(e => e.Id) : 0;

            if (typeof(T) == typeof(ClipInfo))
                id = history.ClipList.Any() ? history.ClipList.Max(e => e.Id) : 0;

            if (typeof(T) == typeof(AlbumInfo))
                id = history.AlbumList.Any() ? history.AlbumList.Max(e => e.Id) : 0;

            if (typeof(T) == typeof(ConcertInfo))
                id = history.ConcertList.Any() ? history.ConcertList.Max(e => e.Id) : 0;

            return id + 1;
        }
    }

    public static class Extension
    {
        public static void AddProduction<T>(this List<T> productionList, T info, Action<T> newProductionEvent) where T : ProductionBase
        {
            productionList.Add(info);
            newProductionEvent.Invoke(info);
        }
    }
}