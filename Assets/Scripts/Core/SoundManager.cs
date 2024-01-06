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
        [SerializeField] private AudioSource sfx;
        [SerializeField] private UISoundSettings _uiSoundSettings;

        /// <summary>
        /// Воспроизводит единичный звук 
        /// </summary>
        public void PlaySound(UIActionType actionType)
        {
            var sound = _uiSoundSettings.GetSound(actionType);
            
            if (sound == null)
                return;
            
            sfx.PlayOneShot(sound);
        }
    }
}