using Game.Settings.Sub;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Player Settings")]  public PlayerSettings  Player;
        [Header("Team Settings")]    public TeamSettings    Team;
        [Header("Track Settings")]   public TrackSettings   Track;
        [Header("Album Settings")]   public AlbumSettings   Album;
        [Header("Clip Settings")]    public ClipSettings    Clip;
        [Header("Concert Settings")] public ConcertSettings Concert;
        [Header("Socials Settings")] public SocialsSettings Socials;
        [Header("Battle Settings")]  public BattleSettings  Battle;
        [Header("Rappers Settings")] public RappersSettings Rappers;
        [Header("Labels Settings")]  public LabelsSettings  Labels;
    }
}