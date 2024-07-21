using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class SpritesManager : Singleton<SpritesManager>
    {
        [SerializeField]
        private Sprite[] images = Array.Empty<Sprite>();

        private Dictionary<string, Sprite> _imagesMap = new();

        public bool TryGetByName(string spriteName, out Sprite sprite)
        {
            if (_imagesMap.TryGetValue(spriteName, out var s))
            {
                sprite = s;
                return true;
            }

            sprite = null;
            return false;
        }

        public Sprite GetRandom()
        {
            var index = Random.Range(0, _imagesMap.Count);
            return _imagesMap.ElementAt(index).Value;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            images = LoadAllImages();
            _imagesMap = images
                .Where(e => e != null)
                .GroupBy(e => e.name)
                .ToDictionary(k => k.Key, v => v.First());
        }

        private static Sprite[] LoadAllImages()
        {
            var files = AssetDatabase.FindAssets("*", new[] { "Assets/Images" });
            var count = files.Length;

            var res = new Sprite[count];
            for (var n = 0; n < count; n++)
            {
                var path = AssetDatabase.GUIDToAssetPath(files[n]);
                res[n] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            return res;
        }
#endif
    }
}
