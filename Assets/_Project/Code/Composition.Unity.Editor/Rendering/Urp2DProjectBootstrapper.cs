using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RapWay.Composition.Unity.Editor.Rendering
{
    [InitializeOnLoad]
    internal static class Urp2DProjectBootstrapper
    {
        private const string SettingsFolder = "Assets/_Project/Settings";
        private const string RenderingFolder = SettingsFolder + "/Rendering";
        private const string RendererDataPath = RenderingFolder + "/RapWay2DRenderer.asset";
        private const string PipelineAssetPath = RenderingFolder + "/RapWayUniversalRenderPipeline.asset";

        static Urp2DProjectBootstrapper()
        {
            EditorApplication.delayCall += ConfigureIfNeeded;
        }

        [MenuItem("Rap Way/Rendering/Configure URP 2D")]
        private static void ConfigureFromMenu()
        {
            Configure(logToConsole: true);
        }

        [MenuItem("Rap Way/Rendering/Validate URP 2D")]
        private static void ValidateFromMenu()
        {
            ValidateBatchMode();
        }

        public static void ValidateBatchMode()
        {
            Configure(logToConsole: false);

            if (GraphicsSettings.defaultRenderPipeline is not UniversalRenderPipelineAsset)
            {
                throw new InvalidOperationException("Rap Way must use a Universal Render Pipeline asset.");
            }

            if (Shader.Find("Hidden/CC2D Baked Skin") == null || Shader.Find("Hidden/CC2D Baked Armor") == null)
            {
                throw new InvalidOperationException("CharacterCreator2D baked shaders are unavailable in the active render pipeline.");
            }

            Debug.Log("Rap Way URP 2D validation passed.");
        }

        private static void ConfigureIfNeeded()
        {
            UniversalRenderPipelineAsset pipelineAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelineAssetPath);
            if (pipelineAsset != null && GraphicsSettings.defaultRenderPipeline == pipelineAsset)
            {
                return;
            }

            Configure(logToConsole: true);
        }

        private static void Configure(bool logToConsole)
        {
            EnsureFolder(SettingsFolder);
            EnsureFolder(RenderingFolder);

            Renderer2DData rendererData = AssetDatabase.LoadAssetAtPath<Renderer2DData>(RendererDataPath);
            if (rendererData == null)
            {
                rendererData = ScriptableObject.CreateInstance<Renderer2DData>();
                ResourceReloader.ReloadAllNullIn(rendererData, UniversalRenderPipelineAsset.packagePath);
                AssetDatabase.CreateAsset(rendererData, RendererDataPath);
            }

            ConfigureRenderer(rendererData);

            UniversalRenderPipelineAsset pipelineAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelineAssetPath);
            if (pipelineAsset == null)
            {
                pipelineAsset = UniversalRenderPipelineAsset.Create(rendererData);
                AssetDatabase.CreateAsset(pipelineAsset, PipelineAssetPath);
            }

            pipelineAsset.supportsHDR = false;
            pipelineAsset.msaaSampleCount = 1;
            pipelineAsset.supportsCameraDepthTexture = false;
            pipelineAsset.supportsCameraOpaqueTexture = false;

            GraphicsSettings.defaultRenderPipeline = pipelineAsset;
            QualitySettings.renderPipeline = pipelineAsset;
            EditorUtility.SetDirty(rendererData);
            EditorUtility.SetDirty(pipelineAsset);
            AssetDatabase.SaveAssets();

            if (logToConsole)
            {
                Debug.Log("Rap Way URP 2D configuration completed.");
            }
        }

        private static void ConfigureRenderer(Renderer2DData rendererData)
        {
            SerializedObject serializedRendererData = new SerializedObject(rendererData);
            SerializedProperty depthStencilBuffer = serializedRendererData.FindProperty("m_UseDepthStencilBuffer");
            if (depthStencilBuffer != null)
            {
                depthStencilBuffer.boolValue = false;
            }

            serializedRendererData.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            string folderName = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
