using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scenes
{
    [CreateAssetMenu(
        fileName = "New ScenesControllerSettings",
        menuName = "Scenes/ScenesControllerSettings")]
    public class ScenesControllerSettings : SerializedScriptableObject
    {
        [SerializeField] private float _loadingDelay;
        [SerializeField] private float _countFilling;
        
        [BoxGroup("Fade times")] [SerializeField] private float _fadeTimeStart;
        [BoxGroup("Fade times")] [SerializeField] private float _fadeTimeShow;
        [BoxGroup("Fade times")] [SerializeField] private float _fadeTimeHide;
        [BoxGroup("Fade times")] [SerializeField] private float _fadeTimeEnd;
        
        [SerializeField] private Dictionary<SceneTypes, string> _sceneNames;

        public string GetSceneName(SceneTypes sceneType)
        {
            if (_sceneNames.TryGetValue(sceneType, out var sceneName)) return sceneName;
            throw new NotImplementedException($"ScenesControllerSettings dont have settings to {sceneType}!");
        }
        
        public float LoadingDelay => _loadingDelay;
        public float CountFilling => _countFilling;
        public float FadeTimeStart => _fadeTimeStart;
        public float FadeTimeShow => _fadeTimeShow;
        public float FadeTimeHide => _fadeTimeHide;
        public float FadeTimeEnd => _fadeTimeEnd;
    }
}