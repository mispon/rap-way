using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Enums;
using UnityEngine;

namespace Scenes
{
    [CreateAssetMenu(
        fileName = "New ScenesControllerSettings",
        menuName = "Scenes/ScenesControllerSettings")]
    public class ScenesControllerSettings : SerializedScriptableObject
    {
        [SerializeField] private float loadingDelay;
        [BoxGroup("Fade times")] [SerializeField] private float fadeTimeStart;
        [BoxGroup("Fade times")] [SerializeField] private float fadeTimeEnd;
        
        [SerializeField] private Dictionary<SceneType, string> _sceneNames;

        public string GetSceneName(SceneType sceneType)
        {
            if (_sceneNames.TryGetValue(sceneType, out var sceneName)) 
                return sceneName;
            
            throw new NotImplementedException($"ScenesControllerSettings dont have settings to {sceneType}!");
        }
        
        public float LoadingDelay => loadingDelay;
        public float FadeTimeStart => fadeTimeStart;
        public float FadeTimeEnd => fadeTimeEnd;
    }
}