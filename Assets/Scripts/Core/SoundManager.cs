using System.Collections;
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
        [SerializeField] private AudioClip[] ambientClips;
        [Space]
        [SerializeField] private AudioClip click;
        [SerializeField] private AudioClip train;
        [SerializeField] private AudioClip pay;
        [SerializeField] private AudioClip levelUp;
        [SerializeField] private AudioClip notify;
        [SerializeField] private AudioClip switcher;
        [SerializeField] private AudioClip workPoint;
        [SerializeField] private AudioClip unlock;
        [SerializeField] private AudioClip achive;

        private int _ambientIndex;
        private Coroutine _ambientSoundRoutine;
        private bool _noSound;

        /// <summary>
        /// Устанавливает значения из настроек 
        /// </summary>
        public void Setup(float volume, bool noSound)
        {
            _ambientIndex = Random.Range(0, ambientClips.Length);
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
                StopCoroutine(_ambientSoundRoutine);
            else
                _ambientSoundRoutine = StartCoroutine(AmbientSoundRoutine());
        }

        /// <summary>
        /// Устанавливает настройку громкости 
        /// </summary>
        public void SetVolume(float volume)
        {
            ambient.volume = volume * 0.3f;
            sfx.volume = volume;
        }

        /// <summary>
        /// Воспроизводит единичный звук 
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (_noSound || clip is null)
                return;
            
            sfx.pitch = Random.Range(0.9f, 1.1f);
            sfx.PlayOneShot(clip);
        }

        /// <summary>
        /// Корутина цикличного воспроизведения фоновой музыки 
        /// </summary>
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator AmbientSoundRoutine()
        {
            if (ambientClips.Length == 0)
                yield break;
            
            ambient.clip = ambientClips[_ambientIndex];
            ambient.Play();
            
            yield return new WaitForSeconds(ambient.clip.length);

            _ambientIndex++;
            _ambientIndex %= ambientClips.Length;

            yield return AmbientSoundRoutine();
        }

        #region SOUND EVENTS

        public void PlayClick() => PlaySound(click);
        public void PlayTrain() => PlaySound(train);
        public void PlayPayment() => PlaySound(pay);
        public void PlayLevelUp() => PlaySound(levelUp);
        public void PlayNotify() => PlaySound(notify);
        public void PlaySwitcher() => PlaySound(switcher);
        public void PlayWorkPoint() => PlaySound(workPoint);
        public void PlayUnlock() => PlaySound(unlock);
        public void PlayAchieve() => PlaySound(achive);

        #endregion
    }
}