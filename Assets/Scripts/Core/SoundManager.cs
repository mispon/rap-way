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
        [SerializeField] private AudioClip click;
        [SerializeField] private AudioClip train;
        [SerializeField] private AudioClip pay;
        [SerializeField] private AudioClip levelUp;
        [SerializeField] private AudioClip notify;
        [SerializeField] private AudioClip switcher;
        [SerializeField] private AudioClip workPoint;
        [SerializeField] private AudioClip unlock;
        [SerializeField] private AudioClip achieve;
        [SerializeField] private AudioClip gameEnd;
        
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
        private void PlaySound(AudioClip clip)
        {
            if (clip == null)
                return;

            sfx.PlayOneShot(clip);
        }

        #region SOUND EVENTS

        public void PlayClick() => PlaySound(click);
        public void PlayTrain() => PlaySound(train);
        public void PlayPayment() => PlaySound(pay);
        public void PlayLevelUp() => PlaySound(levelUp);
        public void PlayNotify() => PlaySound(notify);
        public void PlaySwitch() => PlaySound(switcher);
        public void PlayWorkPoint() => PlaySound(workPoint);
        public void PlayUnlock() => PlaySound(unlock);
        public void PlayAchieve() => PlaySound(achieve);
        public void PlayGameEnd() => PlaySound(gameEnd);

        #endregion
    }
}