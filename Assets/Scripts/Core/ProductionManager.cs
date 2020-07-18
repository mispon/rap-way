using System;
using Game;
using Models.Info.Production;
using Models.Player;
using Utils;

namespace Core
{
    public class ProductionManager: Singleton<ProductionManager>
    {
        public event Action<TrackInfo> onTrackAdd = info => { }; 
        public event Action<AlbumInfo> onAlbumAdd = info => { }; 
        public event Action<ClipInfo> onClipAdd = info => { }; 
        public event Action<ConcertInfo> onConcertAdd = info => { };
        
        private static PlayerHistory playerHistory => PlayerManager.Data.History;
        
        public static void AddProduction<T>(T production) where T : Production
        {
            if (typeof(T) == typeof(TrackInfo))
            {
                var info = production as TrackInfo;
                playerHistory.TrackList.Add(info);
                Instance.onTrackAdd(info);
            }
            else if (typeof(T) == typeof(ClipInfo))
            {
                var info = production as ClipInfo;
                playerHistory.ClipList.Add(info);
                Instance.onClipAdd(info);
            }
            else if (typeof(T) == typeof(AlbumInfo))
            {
                var info = production as AlbumInfo;
                playerHistory.AlbumList.Add(info);
                Instance.onAlbumAdd(info);
            }
            else if (typeof(T) == typeof(ConcertInfo))
            {
                var info = production as ConcertInfo;
                playerHistory.ConcertList.Add(info);
                Instance.onConcertAdd(info);
            }
        }
    }
}