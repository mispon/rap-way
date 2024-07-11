using System;
using System.Linq;
using Core;
using UI.Enums;
using UnityEngine;

namespace Scenes
{
    [Serializable]
    public class SceneTuple
    {
        public SceneType Type;
        public string    Value;
    }

    [CreateAssetMenu(
        fileName = "New ScenesControllerSettings",
        menuName = "Scenes/ScenesControllerSettings")]
    public class ScenesControllerSettings : ScriptableObject
    {
        [SerializeField] private float loadingDelay;
        [SerializeField] private float fadeTimeStart;
        [SerializeField] private float fadeTimeEnd;

        [SerializeField] private SceneTuple[] sceneNames;

        public string GetSceneName(SceneType sceneType)
        {
            var sn = sceneNames.FirstOrDefault(e => e.Type == sceneType);
            if (sn != null)
            {
                return sn.Value;
            }

            throw new RapWayException($"Unexpected scene type: {sceneType}");
        }

        public float LoadingDelay  => loadingDelay;
        public float FadeTimeStart => fadeTimeStart;
        public float FadeTimeEnd   => fadeTimeEnd;
    }
}