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

        [Header("Звуковые клипы")] 
        [SerializeField] private AudioClip buttonClick;

        private bool _noSound;

        /// <summary>
        /// Устанавливает значения из настроек 
        /// </summary>
        public void Setup(float volume, bool noSound)
        {
            SetSound(noSound);
            SetVolume(volume);
        }

        /// <summary>
        /// Устанавливает настройку звука 
        /// </summary>
        public void SetSound(bool value)
        {
            _noSound = value;
            
            if (_noSound)
                ambient.Stop();
            else
                ambient.Play();
        }

        /// <summary>
        /// Устанавливает настройку громкости 
        /// </summary>
        public void SetVolume(float volume)
        {
            ambient.volume = volume * 0.5f;
            sfx.volume = volume;
        }

        /// <summary>
        /// Воспроизводит единичный звук 
        /// </summary>
        public void PlayOne(AudioClip clip)
        {
            if (_noSound)
                return;
            
            sfx.pitch = Random.Range(0.8f, 1.2f);
            sfx.PlayOneShot(clip);
        }

        /// <summary>
        /// Воспроизводит звук клика кнопки
        /// </summary>
        public void Click()
        {
            sfx.PlayOneShot(buttonClick);
        }
    }
}