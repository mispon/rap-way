using ScriptableObjects;
using UnityEngine;
using UnityEngine.Audio;

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

        private const float realMinVolume = -80;
        private const float minVolume = -30;
        private const float maxVolume = 0;
        
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

        public void SetVolume(string volumeKey, float volume)
        {
            if (volume <= minVolume)
            {
                volume = realMinVolume;
            }
            
            audioMixerGroup.audioMixer.SetFloat(volumeKey, volume);
        }
        
        private void LoadVolume(string volumeKey)
        {
            if (!PlayerPrefs.HasKey(volumeKey))
            {
                PlayerPrefs.SetFloat(volumeKey, minVolume / 2);
            }
            
            var volume = PlayerPrefs.GetFloat(volumeKey);
            SetVolume(volumeKey, volume);
        }
    }
}