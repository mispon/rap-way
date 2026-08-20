using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RapWay.EditorTools
{
    public static class Stage4UiToolkitRepairTool
    {
        private const string CamerasPrefabPath = "Assets/_Project/Prefabs/Cameras.prefab";
        private const string PanelSettingsAssetPath = "Assets/_Project/Resources/UI/AppShellPanelSettings.asset";
        private const float SafeNearClipPlane = 0.3f;
        private const float DefaultFarClipPlane = 100f;
        private const float DefaultOrthographicSize = 17f;
        private static readonly Color DefaultBackgroundColor = new(0.0627451f, 0.07058824f, 0.09411765f, 1f);

        [MenuItem("Rap Way/Stage 4/Repair UI Toolkit Shell")]
        public static void RepairFromMenu()
        {
            RepairInternal();
        }

        public static void RepairFromBatchMode()
        {
            try
            {
                RepairInternal();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
                return;
            }

            EditorApplication.Exit(0);
        }

        private static void RepairInternal()
        {
            EnsurePanelSettingsAsset();
            RepairCamerasPrefab();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsurePanelSettingsAsset()
        {
            EnsureFolder("Assets/_Project/Resources");
            EnsureFolder("Assets/_Project/Resources/UI");

            PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsAssetPath);
            if (panelSettings == null)
            {
                panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                AssetDatabase.CreateAsset(panelSettings, PanelSettingsAssetPath);
            }

            panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            panelSettings.scale = 1f;
            panelSettings.sortingOrder = 100;

            EditorUtility.SetDirty(panelSettings);
        }

        private static void RepairCamerasPrefab()
        {
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(CamerasPrefabPath);
            try
            {
                prefabRoot.name = "Main Camera";
                prefabRoot.tag = "MainCamera";
                prefabRoot.transform.localPosition = new Vector3(0f, 0f, -10f);
                prefabRoot.transform.localRotation = Quaternion.identity;
                prefabRoot.transform.localScale = Vector3.one;

                for (int i = prefabRoot.transform.childCount - 1; i >= 0; i--)
                {
                    UnityEngine.Object.DestroyImmediate(prefabRoot.transform.GetChild(i).gameObject);
                }

                Camera mainCamera = prefabRoot.GetComponent<Camera>();
                if (mainCamera == null)
                {
                    mainCamera = prefabRoot.AddComponent<Camera>();
                }

                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = DefaultBackgroundColor;
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = DefaultOrthographicSize;
                mainCamera.nearClipPlane = SafeNearClipPlane;
                mainCamera.farClipPlane = DefaultFarClipPlane;
                mainCamera.cullingMask = ~0;

                if (prefabRoot.GetComponent<AudioListener>() == null)
                {
                    prefabRoot.AddComponent<AudioListener>();
                }

                PrefabUtility.SaveAsPrefabAsset(prefabRoot, CamerasPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        private static void EnsureFolder(string assetFolderPath)
        {
            if (AssetDatabase.IsValidFolder(assetFolderPath))
            {
                return;
            }

            int separatorIndex = assetFolderPath.LastIndexOf('/');
            if (separatorIndex <= 0)
            {
                throw new InvalidOperationException($"Cannot create asset folder '{assetFolderPath}'.");
            }

            string parentFolder = assetFolderPath.Substring(0, separatorIndex);
            string folderName = assetFolderPath.Substring(separatorIndex + 1);

            EnsureFolder(parentFolder);
            AssetDatabase.CreateFolder(parentFolder, folderName);
        }
    }
}
