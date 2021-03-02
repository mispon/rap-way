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
        [Space, Tooltip("Кривая ценности одного очка работы от количества фанатов")]
        public AnimationCurve AlbumFansToPointsIncomeCurve;
        [Tooltip("Коэффициент совпадения с трендом")]
        public float AlbumTrendsEqualityMultiplier;
        [Space, Tooltip("Зависимость числа слушателей от очков")]
        public AnimationCurve AlbumListenAmountCurve;
        [Tooltip("Зависимость места в чарте от количества слушателей")]
        public AnimationCurve AlbumChartPositionCurve;
        [Space, Tooltip("Зависимость коэффициента прироста фанатов от текущих фанатов")]
        public AnimationCurve AlbumFansIncomeCurve;
        [Tooltip("Коэффициент влияния хайпа на прирост фанатов")]
        public float AlbumHypeImpactMultiplier;
        [Tooltip("Коэффициент заработка от количества прослушиваний")]
        public int AlbumMoneyIncomeMultiplier;

        [Header("CLIP")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int ClipWorkDuration;
        [Space, Tooltip("Кривая ценности одного очка работы от количества фанатов")]
        public AnimationCurve ClipFansToPointsIncomeCurve;
        [Tooltip("Зависимость числа просмотров от очков")]
        public AnimationCurve ClipViewsCurve;
        [Tooltip("Зависимость коэффициента кол-ва просмотров от успеха трека (номер в чарте)")]
        public AnimationCurve ClipViewsFromTrackCurve;
        [Space, Tooltip("Доля лайков в зависимости от импакта трека")]
        public AnimationCurve ClipLikesFromTrackCurve;
        [Range(0f, 1f), Tooltip("Минимальная доля оценок на количество просмотров")]
        public float ClipMinMarksRatio = 0.5f;
        [Range(0f, 1f), Tooltip("Максимальная доля оценок на количество просмотров")]
        public float ClipMaxMarksRatio = 0.7f;
        [Space, Tooltip("Зависимость коэффициента прироста фанатов от текущих фанатов")]
        public AnimationCurve ClipFansIncomeCurve;
        [Tooltip("Коэффициент влияния хайпа на прирост фанатов")]
        public float ClipHypeImpactMultiplier;
        [Tooltip("Коэффициент заработка от количества просмотров")]
        public int ClipMoneyIncomeMultiplier;

        [Header("CONCERT")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int ConcertWorkDuration;
        [Space, Tooltip("Кривая ценности одного очка работы от количества фанатов")]
        public AnimationCurve ConcertFansToPointsIncomeCurve;
        [Tooltip("Зависимость числа продажи билетов от очков")]
        public AnimationCurve ConcertTicketsSoldCurve;
        [Tooltip("Зависимость коэффициента кол-ва продаж билетов от успеха альбома (номер в чарте)")]
        public AnimationCurve ConcertTicketsFromAlbumCurve;
        [Tooltip("Зависимость коэффициента продаж билетов от кол-ва проведенных концертов на тот же альбом")]
        public AnimationCurve ConcertSamePlaceAgainCurve;
        [Tooltip("Коэффициент влияния хайпа на продажу билетов")]
        public float ConcertHypeImpactMultiplier;
        [Tooltip("Зависимость коэффициента прироста фанатов от текущих фанатов")]
        public AnimationCurve ConcertFansIncomeCurve;

        [Header("SOCIALS")]
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int SocialsWorkDuration;
        [Space, Tooltip("Чем больше фанатов, тем больше за тобой следят, тем больше прирост хайпа")]
        public float SocialsFansMultiplier;
        [Tooltip("Зависимость коэффициента от доли потраченных денег")]
        public AnimationCurve SocialsCharityMoneyRatioCurve;
        [Tooltip("Базовый коэффициента импакта хайпа по каждому из соц.действий")]
        public int[] SocialsHypeImpactData;

        [Header("OTHER")]
        public int RappersWorkDuration;
        public int BattleWorkDuration;
        public int FeatWorkDuration;
    }
}