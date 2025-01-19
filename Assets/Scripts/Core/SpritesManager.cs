using System;
using System.Collections.Generic;
using System.IO;
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

        [SerializeField]
        private Sprite[] portraits = Array.Empty<Sprite>();

        private Dictionary<string, Sprite> _imagesMap = new();

        private void Start()
        {
            _imagesMap = images
                .Where(e => e != null)
                .GroupBy(e => e.name)
                .ToDictionary(k => k.Key, v => v.First());

            portraits = LoadPortraits();
        }

        public bool TryGetByName(string spriteName, out Sprite sprite)
        {
            if (spriteName != "" && spriteName != "null")
            {
                if (_imagesMap.TryGetValue(spriteName, out var s))
                {
                    sprite = s;
                    return true;
                }
            }

            sprite = null;
            return false;
        }

        public Sprite GetRandom()
        {
            var index = Random.Range(0, _imagesMap.Count);
            return _imagesMap.ElementAt(index).Value;
        }

        public Sprite GetPortrait(string nickname)
        {
            var filename = $"{nickname.ToLower()}.png";
            return portraits.FirstOrDefault(e => e.name == filename);
        }

        public void AppendPortrait(Sprite sprite)
        {
            var newArr = portraits.ToList();
            newArr.Add(sprite);
            portraits = newArr.ToArray();
        }

        private static Sprite[] LoadPortraits()
        {
            var basePath = Path.Combine(Application.persistentDataPath, "Portraits/");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            var files = Directory.GetFiles(basePath, "*.png");
            var count = files.Length;

            var res = new Sprite[count];
            for (var n = 0; n < count; n++)
            {
                res[n] = GetSpriteFromImage(files[n]);
            }

            return res;
        }

        private static Sprite GetSpriteFromImage(string imgPath)
        {
            // Converts desired path into byte array
            var pngBytes = File.ReadAllBytes(imgPath);

            // Creates texture and loads byte array data to create image
            var tex = new Texture2D(2, 2);
            tex.LoadImage(pngBytes);

            // Creates a new Sprite based on the Texture2D
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

            var lastIndex = imgPath.LastIndexOf("/", StringComparison.Ordinal);
            sprite.name = imgPath[(lastIndex + 1)..];

            return sprite;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            images = LoadAllImages();
        }

        private static Sprite[] LoadAllImages()
        {
            var files = AssetDatabase.FindAssets("*", new[] {"Assets/Images"});
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