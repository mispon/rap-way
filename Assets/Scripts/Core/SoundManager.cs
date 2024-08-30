using ScriptableObjects;
using UnityEngine;
using UnityEngine.Audio;

namespace Core
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        [SerializeField] private AudioSource sfx;
        [SerializeField] private UISoundSettings _uiSoundSettings;

        private const string masterVolumeKey = "MasterVolume";
        private const string musicVolumeKey = "MusicVolume";

        private const float realMinVolume = -80;
        private const float minVolume = -30;
        private const float maxVolume = 0;

        private float _currentMusicVolume;

        private void Start()
        {
            LoadVolume(masterVolumeKey);
            LoadVolume(musicVolumeKey);
        }

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
            _currentMusicVolume = volume;
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

        public void PauseMusic()
        {
            if (audioMixerGroup.audioMixer.GetFloat(musicVolumeKey, out float vol) && vol != realMinVolume)
            {
                _currentMusicVolume = vol;
                audioMixerGroup.audioMixer.SetFloat(musicVolumeKey, realMinVolume);
            }
        }

        public void UnpauseMusic()
        {
            audioMixerGroup.audioMixer.SetFloat(musicVolumeKey, _currentMusicVolume);
        }
    }
}