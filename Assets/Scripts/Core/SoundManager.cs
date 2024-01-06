using Data;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

namespace Core
{
    /// <summary>
    /// Логика управления звуком
    /// </summary>
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        [SerializeField] private AudioSource sfx;
        [SerializeField] private UISoundSettings _uiSoundSettings;

        private void Start()
        {
            LoadVolume("MasterVolume");
            LoadVolume("MusicVolume");
        }
        
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

        private void LoadVolume(string volumeKey)
        {
            if (PlayerPrefs.HasKey(volumeKey) is false)
            {
                const float maxVolume = 0;
                PlayerPrefs.SetFloat(volumeKey, maxVolume);
            }
            
            var volume = PlayerPrefs.GetFloat(volumeKey);
            audioMixerGroup.audioMixer.SetFloat(volumeKey, volume);
        }
    }
}