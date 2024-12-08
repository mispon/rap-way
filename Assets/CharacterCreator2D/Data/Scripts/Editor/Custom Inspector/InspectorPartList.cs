using System.Collections.Generic;
using System.IO;
using CharacterCreator2D;
using UnityEditor;
using UnityEngine;

namespace CharacterEditor2D
{
    [CustomEditor(typeof(PartList))]
    public class InspectorPartList : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("refresh"))
            {
                refresh();
            }
        }

        private void refresh()
        {
            var partlist = (PartList) target;
            if (partlist == null)
            {
                return;
            }

            RefreshPartPackage();
            Refresh(partlist);
        }

        public static void Refresh(PartList partList)
        {
            if (partList == null)
            {
                return;
            }

            partList.partPacks = new List<PartPack>();
            var sourcepath = GetSourcePath();
            foreach (var s in sourcepath)
            {
                var parts = EditorUtils.GetScriptables<Part>(s, true);
                foreach (var p in parts)
                {
                    var tpack = GetPack(p.category, partList.partPacks);
                    if (tpack == null)
                    {
                        tpack          = new PartPack();
                        tpack.category = p.category;
                        partList.partPacks.Add(tpack);
                    }

                    tpack.parts.Add(p);
                }
            }

            EditorUtility.SetDirty(partList);
            PartRefCustomMenu.RefreshPartReferers();
        }

        public static void RefreshPartPackage()
        {
            var sourcepath = GetSourcePath();
            foreach (var s in sourcepath)
            {
                var packagename = Path.GetFileNameWithoutExtension(s);
                var parts       = EditorUtils.GetScriptables<Part>(s, true);
                foreach (var p in parts)
                {
                    if (p.packageName != packagename)
                    {
                        p.packageName = packagename;
                        EditorUtility.SetDirty(p);
                    }
                }
            }
        }

        public static List<string> GetSourcePath()
        {
            var val         = new List<string>();
            var directories = Directory.GetDirectories(WizardUtils.PartFolder);
            foreach (var d in directories)
            {
                val.Add(d);
            }

            return val;
        }

        public static PartPack GetPack(PartCategory category, List<PartPack> pack)
        {
            foreach (var p in pack)
            {
                if (p.category == category)
                {
                    return p;
                }
            }

            return null;
        }
    }
}