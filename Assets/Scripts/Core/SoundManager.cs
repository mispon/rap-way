using Data;
using UnityEngine;
using Utils;

namespace Core
{
    /// <summary>
    /// Логика управления звуком
    /// </summary>
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("Источники звука")]
        [SerializeField] private AudioSource ambient;
        [SerializeField] private AudioSource sfx;

        [SerializeField] private UISoundSettings _uiSoundSettings;

        /// <summary>
        /// Воспроизводит единичный звук 
        /// </summary>
        public void PlaySound(UIActionType actionType)
        {
            sfx?.PlayOneShot(_uiSoundSettings.GetSound(actionType));
        }
    }
}