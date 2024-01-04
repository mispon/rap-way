using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(
        fileName = "New UISoundSettings",
        menuName = "Audio/UISoundSettings")]
    public class UISoundSettings : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<UIActionType, AudioClip> _uiSoundSettings;

        public AudioClip GetSound(UIActionType actionType)
        {
            if (_uiSoundSettings.TryGetValue(actionType, out var sound)) return sound;
            throw new NotImplementedException($"UISoundSettings dont have settings {actionType}!");
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
        GameEnd  = 10,
    }
}