using UnityEngine;

namespace Core.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("TRACK")]
        public int TrackWorkDuration;

        [Header("ALBUM")]
        public int AlbumWorkDuration;

        [Header("CLIP")]
        public int ClipWorkDuration;

        [Header("CONCERT")]
        public int ConcertWorkDuration;

        [Header("BATTLE")]
        public int BattleWorkDuration;

        [Header("FEAT")]
        public int FeatWorkDuration;

        [Header("SOCIALS")]
        public int SocialsWorkDuration;

        [Header("OTHER")]
        public int RappersWorkDuration;
    }
}