using Game.Settings.Sub;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings")]
    public class GameSettings : ScriptableObject
    {
        [BoxGroup("Player Settings")]  public PlayerSettings  Player;
        [BoxGroup("Team Settings")]    public TeamSettings    Team;
        [BoxGroup("Track Settings")]   public TrackSettings   Track;
        [BoxGroup("Album Settings")]   public AlbumSettings   Album;
        [BoxGroup("Clip Settings")]    public ClipSettings    Clip;
        [BoxGroup("Concert Settings")] public ConcertSettings Concert;
        [BoxGroup("Socials Settings")] public SocialsSettings Socials;
        [BoxGroup("Battle Settings")]  public BattleSettings  Battle;
        [BoxGroup("Rappers Settings")] public RappersSettings Rappers;
        [BoxGroup("Labels Settings")]  public LabelsSettings  Labels;
    }
}