using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;

namespace CharacterEditor2D
{
    public static class SpriteSlicerUtils
    {
        public static List<Sprite> SliceSprite(string sourcePath, string targetPath, params string[] excludedName)
        {
            try
            {
                TextureImporter sourceti = (TextureImporter)AssetImporter.GetAtPath(sourcePath);
                TextureImporter targetti = (TextureImporter)AssetImporter.GetAtPath(targetPath);

                //..reset
                targetti.spriteImportMode = SpriteImportMode.Single;
                targetti.SaveAndReimport();
                //reset..

                targetti.spriteImportMode = sourceti.spriteImportMode;
                List<SpriteMetaData> tempsheet = new List<SpriteMetaData>();

                // foreach (SpriteMetaData m in sourceti.spritesheet)
                // {
                //     if (contains(m.name, excludedName))
                //         continue;

                //     SpriteMetaData tempsmd = new SpriteMetaData();
                //     tempsmd.alignment = m.alignment;
                //     tempsmd.border = new Vector4(m.border.x, m.border.y, m.border.z, m.border.w);
                //     tempsmd.name = m.name;
                //     tempsmd.pivot = new Vector2(m.pivot.x, m.pivot.y);
                //     tempsmd.rect = new Rect(m.rect);
                //     tempsheet.Add(tempsmd);
                // }

                var sourceFactory = new SpriteDataProviderFactories();
                sourceFactory.Init();
                var sourceData = sourceFactory.GetSpriteEditorDataProviderFromObject(sourceti);
                sourceData.InitSpriteEditorDataProvider();
                var sourceRects = sourceData.GetSpriteRects();

                var newRects = new List<SpriteRect>();
                foreach (var sr in sourceRects)
                {
                    if (contains(sr.name, excludedName))
                        continue;
                    newRects.Add(sr);
                }

                var targetFactory = new SpriteDataProviderFactories();
                targetFactory.Init();
                var targetData = targetFactory.GetSpriteEditorDataProviderFromObject(targetti);
                targetData.InitSpriteEditorDataProvider();

                targetData.SetSpriteRects(newRects.ToArray());
                targetData.Apply();
                targetti.SaveAndReimport();

                Object[] tobj = AssetDatabase.LoadAllAssetsAtPath(targetPath);
                List<Sprite> val = new List<Sprite>();
                foreach (Object o in tobj)
                {
                    if (o is Sprite)
                        val.Add((Sprite)o);
                }

                return val;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return null;
            }
        }

        private static bool contains(string value, string[] listVal)
        {
            foreach (string v in listVal)
            {
                if (value == v)
                    return true;
            }

            return false;
        }

        public static List<string> GetSlicedNames(Texture2D texture)
        {
            if (texture == null)
                return new List<string>();

            List<string> val = new List<string>();
            TextureImporter tempti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));

            var sourceFactory = new SpriteDataProviderFactories();
            sourceFactory.Init();
            var sourceData = sourceFactory.GetSpriteEditorDataProviderFromObject(tempti); ;
            sourceData.InitSpriteEditorDataProvider();
            var sourceRects = sourceData.GetSpriteRects();

            foreach (SpriteRect sr in sourceRects)
                val.Add(sr.name);

            return val;
        }
    }
}