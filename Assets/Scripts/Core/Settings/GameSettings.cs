using UnityEngine;

namespace Core.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("PLAYER")]
        [Range(100_000_000, 1_000_000_000), Tooltip("Максимальное кол-во фанатов, шт")]
        public int MaxFans;
        [Range(100_000_000, 1_500_000_000), Tooltip("Максимальное кол-во фанатов, шт")]
        public int MaxMoney;
        [Range(500, 5000), Tooltip("Базовое значение фанатов в анализаторе")]
        public int BaseFans;

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
        [Range(0.001f, 0.01f), Tooltip("Цена одного прослушивания")]
        public float TrackListenCost;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int TrackRewardExp;

        [Header("ALBUM")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int AlbumWorkDuration;
        [Range(0.1f, 0.8f), Tooltip("Базовая величина хайпа, %")]
        public float AlbumBaseHype;
        [Range(0.1f, 0.7f), Tooltip("Базовое качество трека, %")]
        public float AlbumBaseQuality;
        [Range(10, 400), Tooltip("Максимальное количество очков работы, шт")]
        public int AlbumWorkPointsMax;
        [Tooltip("Зависимость оценки от качества трека")]
        public AnimationCurve AlbumGradeCurve;
        [Tooltip("Зависимость позиции в чарте от прослушиваний")]
        public AnimationCurve AlbumChartCurve;
        [Range(0f, 1f), Tooltip("Вероятность хита, %")]
        public float AlbumHitChance;
        [Tooltip("Зависимость изменения фанатов от качества трека, %")]
        public AnimationCurve AlbumFansIncomeCurve;
        [Range(0.001f, 0.01f), Tooltip("Цена одного прослушивания")]
        public float AlbumListenCost;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int AlbumRewardExp;

        [Header("CLIP")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int ClipWorkDuration;
        [Tooltip("Зависимость оценки от качества клипа")]
        public AnimationCurve ClipGradeCurve;
        [Range(10, 300), Tooltip("Максимальное количество очков работы, шт")]
        public int ClipWorkPointsMax;
        [Range(0f, 1f), Tooltip("Вероятность хита, %")]
        public float ClipHitChance;
        [Range(0f, 1f), Tooltip("Сила влияния количества прослушиваний трека")]
        public float ClipTrackListensImpact;
        [Range(0f, 1f), Tooltip("Доля активных зрителей, %")]
        public float ClipActiveViewers;
        [Tooltip("Зависимость изменения фанатов от качества клипа, %")]
        public AnimationCurve ClipFansIncomeCurve;
        [Range(0.001f, 0.01f), Tooltip("Цена одного просмотра")]
        public float ClipViewCost;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int ClipRewardExp;

        [Header("CONCERT")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int ConcertWorkDuration;
        [Range(1, 60), Tooltip("Длительность отдыха после концерта, дни")]
        public int ConcertCooldown;
        [Tooltip("Зависимость оценки от качества концерта")]
        public AnimationCurve ConcertGradeCurve;
        [Range(10, 400), Tooltip("Максимальное количество очков работы, шт")]
        public int ConcertWorkPointsMax;
        [Range(0f, 1f), Tooltip("Сила влияния количества прослушиваний альбома")]
        public float ConcertAlbumListensImpact;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int ConcertRewardExp;

        [Header("SOCIALS")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int SocialsWorkDuration;
        [Range(1, 30), Tooltip("Длительность отдыха PR-менеджера, дни")]
        public int SocialsCooldown;
        [Range(10, 100), Tooltip("Максимальное количество очков работы, шт")]
        public int SocialsWorkPointsMax;
        [Range(0f, 1f), Tooltip("Сила влияния количества денег при пожертвовании")]
        public float SocialsCharitySizeImpact;
        [Range(0f, 1f), Tooltip("Доля активных фанатов от общего кол-ва, %")]
        public float SocialsActiveFansGroup;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int SocialsRewardExp;
        [Range(1, 1000), Tooltip("Минимальная сумма баланса для возможности сделать пожертвование")]
        public int MinBalanceForCharity;

        [Header("BATTLE")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int BattleWorkDuration;
        [Tooltip("Зависимость изменения количества фанатов")]
        public AnimationCurve BattleFansChange;
        [Range(1, 100), Tooltip("Количество очков хайпа за победу")]
        public int BattleWinnerHype;
        [Range(1, 100), Tooltip("Количество очков хайпа за проигрыш")]
        public int BattleLoserHype;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int BattleRewardExp;

        [Header("OTHER")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int RappersWorkDuration;
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int FeatWorkDuration;
        [Range(50_000, 300_000), Tooltip("Минимальное кол-во фанатов для участия в чартах")]
        public int MinFansForCharts;
        [Range(1, 30), Tooltip("Длительность отдыха менеджера, дни")]
        public int ManagerCooldown;
    }
}