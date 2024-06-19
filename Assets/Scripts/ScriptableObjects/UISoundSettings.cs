using System;
using System.Linq;
using Core;
using Core.PropertyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScriptableObjects
{
    [Serializable]
    public class SoundTuple
    {
        public UIActionType Type;
        public AudioClip Value;
    }
    
    [CreateAssetMenu(
        fileName = "New UISoundSettings",
        menuName = "Audio/UISoundSettings")]
    public class UISoundSettings : ScriptableObject
    {
        [ArrayElementTitle(new []{ "Type" })]
        [SerializeField] private SoundTuple[] sounds;

        public AudioClip GetSound(UIActionType actionType)
        {
            var soundsArr = sounds
                .Where(e => e.Type == actionType)
                .Select(e => e.Value)
                .ToArray();
            
            if (soundsArr.Length == 0)
                throw new RapWayException($"Sounds for action {actionType} not set!");

            int idx = Random.Range(0, soundsArr.Length);
            return soundsArr[idx];
        }
    }
    
    public enum UIActionType
    {
        Click     = 0,
        Train     = 1,
        Pay       = 3,
        LevelUp   = 4,
        Notify    = 5,
        Switcher  = 6,
        WorkPoint = 7,
        Unlock    = 8,
        Achieve   = 9,
        GameEnd  = 10
    }
}