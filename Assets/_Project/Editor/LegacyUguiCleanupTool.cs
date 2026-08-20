using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RapWay.EditorTools
{
    public static class LegacyUguiCleanupTool
    {
        private static readonly string[] ScenePaths =
        {
            "Assets/_Project/Scenes/Boot.unity",
            "Assets/_Project/Scenes/Game.unity",
            "Assets/_Project/Scenes/Debug.unity"
        };

        private static readonly string[] AssetPathsToDelete =
        {
            "Assets/_Project/Code/App/Installers/UIInstaller.cs",
            "Assets/_Project/Code/Core/UI",
            "Assets/_Project/Code/Data/UIConfig.cs",
            "Assets/_Project/Code/UI",
            "Assets/_Project/Code/Features/Test/UI",
            "Assets/_Project/Data/UI",
            "Assets/_Project/Prefabs/UI"
        };

        [MenuItem("Rap Way/Stage 4/Cleanup Legacy uGUI")]
        public static void Cleanup()
        {
            try
            {
                CleanupRootLifetimeScopePrefab();
                CleanupScenes();
                DeleteLegacyAssets();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                throw;
            }
        }

        public static void CleanupFromBatchMode()
        {
            Cleanup();
        }

        private static void CleanupRootLifetimeScopePrefab()
        {
            const string prefabPath = "Assets/_Project/Prefabs/RootLifetimeScope.prefab";
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        private static void CleanupScenes()
        {
            foreach (string scenePath in ScenePaths)
            {
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                RemoveLegacyObjects(scenePath, scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        private static void RemoveLegacyObjects(string scenePath, Scene scene)
        {
            HashSet<string> namesToDelete = new(StringComparer.Ordinal);
            namesToDelete.Add("Canvas");
            namesToDelete.Add("EventSystem");

            if (scenePath.EndsWith("Game.unity", StringComparison.Ordinal))
            {
                namesToDelete.Add("UI Root");
            }

            if (scenePath.EndsWith("Debug.unity", StringComparison.Ordinal))
            {
                namesToDelete.Add("UI");
            }

            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (!namesToDelete.Contains(rootObject.name))
                {
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(rootObject);
            }
        }

        private static void DeleteLegacyAssets()
        {
            foreach (string assetPath in AssetPathsToDelete)
            {
                if (!AssetDatabase.DeleteAsset(assetPath))
                {
                    if (File.Exists(assetPath) || Directory.Exists(assetPath))
                    {
                        throw new InvalidOperationException($"Failed to delete legacy asset '{assetPath}'.");
                    }
                }
            }
        }
    }
}
