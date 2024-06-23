using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.SocialNetworks.Email
{
    public partial class EmailManager
    {
        [SerializeField]
        private Sprite[] images = Array.Empty<Sprite>();

        private Dictionary<string, Sprite> imagesMap = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            images = LoadAllImages();
            imagesMap = images
                .Where(e => e != null)
                .GroupBy(e => e.name)
                .ToDictionary(k => k.Key, v => v.First());
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