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
        [SerializeField] private AudioClip buttonClick;

        private int _ambientIndex;
        private Coroutine _ambientSoundRoutine;
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
                StopCoroutine(_ambientSoundRoutine);
            else
                _ambientSoundRoutine = StartCoroutine(AmbientSoundRoutine());
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

        /// <summary>
        /// Корутина цикличного воспроизведения фоновой музыки 
        /// </summary>
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator AmbientSoundRoutine()
        {
            ambient.clip = ambientClips[_ambientIndex];
            ambient.Play();
            
            yield return new WaitForSeconds(ambient.clip.length);

            _ambientIndex++;
            if (_ambientIndex >= ambientClips.Length)
                _ambientIndex = 0;

            yield return AmbientSoundRoutine();
        }
    }
}