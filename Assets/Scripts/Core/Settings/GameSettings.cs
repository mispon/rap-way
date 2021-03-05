using UnityEngine;

namespace Core.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("TRACK")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int TrackWorkDuration;
        [Range(0.1f, 0.8f), Tooltip("Базовая величина хайпа, %")]
        public float TrackBaseHype;
        [Range(0.1f, 0.7f), Tooltip("Базовое качество трека, %")]
        public float TrackBaseQuality;
        [Range(10, 250), Tooltip("Максимальное количество очков работы, шт")]
        public int TrackWorkPointsMax;
        [Tooltip("Зависимость оценки от качества трека")]
        public AnimationCurve TrackGradeCurve;
        [Tooltip("Зависимость позиции в чарте от прослушиваний")]
        public AnimationCurve TrackChartCurve;
        [Range(0f, 1f), Tooltip("Вероятность хита, %")]
        public float TrackHitChance;
        [Tooltip("Зависимость изменения фанатов от качества трека, %")]
        public AnimationCurve TrackFansIncomeCurve;
        [Range(0.1f, 1f), Tooltip("Цена одного прослушивания")]
        public float TrackListenCost;

        [Header("ALBUM")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int AlbumWorkDuration;
        [Range(0.1f, 0.8f), Tooltip("Базовая величина хайпа, %")]
        public float AlbumBaseHype;
        [Range(0.1f, 0.7f), Tooltip("Базовое качество трека, %")]
        public float AlbumBaseQuality;
        [Range(10, 250), Tooltip("Максимальное количество очков работы, шт")]
        public int AlbumWorkPointsMax;
        [Tooltip("Зависимость оценки от качества трека")]
        public AnimationCurve AlbumGradeCurve;
        [Tooltip("Зависимость позиции в чарте от прослушиваний")]
        public AnimationCurve AlbumChartCurve;
        [Range(0f, 1f), Tooltip("Вероятность хита, %")]
        public float AlbumHitChance;
        [Tooltip("Зависимость изменения фанатов от качества трека, %")]
        public AnimationCurve AlbumFansIncomeCurve;
        [Range(0.1f, 1f), Tooltip("Цена одного прослушивания")]
        public float AlbumListenCost;

        [Header("CLIP")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int ClipWorkDuration;
        [Tooltip("Зависимость оценки от качества клипа")]
        public AnimationCurve ClipGradeCurve;
        [Range(10, 400), Tooltip("Максимальное количество очков работы, шт")]
        public int ClipWorkPointsMax;
        [Range(0f, 1f), Tooltip("Вероятность хита, %")]
        public float ClipHitChance;
        [Range(0f, 1f), Tooltip("Сила влияния количества прослушиваний трека")]
        public float ClipTrackListensImpact;
        [Range(0f, 1f), Tooltip("Доля активных зрителей, %")]
        public float ClipActiveViewers;
        [Tooltip("Зависимость изменения фанатов от качества клипа, %")]
        public AnimationCurve ClipFansIncomeCurve;
        [Range(0.1f, 1f), Tooltip("Цена одного просмотра")]
        public float ClipViewCost;

        [Header("CONCERT")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int ConcertWorkDuration;
        [Tooltip("Зависимость оценки от качества концерта")]
        public AnimationCurve ConcertGradeCurve;
        [Range(10, 400), Tooltip("Максимальное количество очков работы, шт")]
        public int ConcertWorkPointsMax;
        [Range(0f, 1f), Tooltip("Сила влияния количества прослушиваний альбома")]
        public float ConcertAlbumListensImpact;

        [Header("SOCIALS")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int SocialsWorkDuration;
        // todo

        [Header("OTHER")]
        public int RappersWorkDuration;
        public int BattleWorkDuration;
        public int FeatWorkDuration;
    }
}