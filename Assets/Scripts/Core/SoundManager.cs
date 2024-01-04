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
        /// Устанавливает значения из настроек 
        /// </summary>
        public void Setup(float soundVolume, float musicVolume)
        {
            SetVolume(soundVolume, musicVolume);
        }

        /// <summary>
        /// Устанавливает настройку громкости 
        /// </summary>
        public void SetVolume(float soundVolume, float musicVolume)
        {
            ambient.volume = musicVolume;
            sfx.volume = soundVolume;
        }

        /// <summary>
        /// Воспроизводит единичный звук 
        /// </summary>
        public void PlaySound(UIActionType actionType)
        {
            sfx?.PlayOneShot(_uiSoundSettings.GetSound(actionType));
        }
    }
}