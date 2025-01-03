﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharacterCreator2D.UI
{
    public class UIExportPNG : MonoBehaviour
    {
        [Header("UI References")]
        public Camera previewCam;
        public Animator        character;
        public CharacterViewer characterViewer;
        public RawImage        previewUI;
        public RectTransform   previewUIContainer;
        public RectTransform   previewScrollRect;
        public Dropdown        baseAnimation;
        public Dropdown        aimAnimation;
        public Dropdown        emoteDropdown;
        public Toggle          playToggle;
        public Text            animationLengthUI;
        public Text            frameCountUI;
        public Dropdown        configPreset;
        public RuntimeDialog   runtimeDialog;
        public Button          buttonExport;
        [Space]
        public InputField inputFPS;
        public InputField inputWidth;
        public InputField inputHeight;
        public Dropdown   inputExportMode;
        public InputField inputOffsetX;
        public InputField inputOffsetY;
        public InputField inputScale;
        public Dropdown   inputScaleMode;

        [Header("Export Settings")]
        public int targetFPS = 12;
        public int             width         = 512;
        public int             height        = 512;
        public BackgroundColor bgColor       = 0;
        public ExportMode      exportMode    = 0;
        public Vector3         charPosition  = Vector3.zero;
        public Vector3         charScale     = Vector3.one;
        public ScaleMode       scaleMode     = 0;
        public bool            flipCharacter = false;
        public bool            loopAimLayer  = false;

        // Path and file name
        [HideInInspector]
        public string path = "";
#if UNITY_EDITOR
        private readonly string fileName = "New Character";
#endif

        // Export config path
        private          string configPath      = "";
        private readonly string configExtension = ".pngconfig";

        private readonly int superSampling = 2;

        private float  targetLength;
        private float  baseLength;
        private float  aimLength;
        private int    frameCount;
        private string selectedAnimation;
        private string selectedAim;

        private int  currentPreset;
        private bool isLoadingconfig;

        private readonly float PPU = 100;

        private RenderTexture renderTexture;

        // Initialize the UI when it is enabled
        private void OnEnable()
        {
            UpdateSpriteSize();
            UpdateZoomPreview(1);
            UpdateCharacter();
            SetCharFlip(flipCharacter);

            GetConfigPath();
            UpdateConfigDropdown();
            InitEmoteDropdown();
            GetAnimationLength();

            LayoutRebuilder.MarkLayoutForRebuild(previewScrollRect);
            LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
        }

        // Reset the character to default, used when exiting the Export menu back to creator UI
        public void ResetCharacter()
        {
            var rotation = Vector3.zero;
            rotation.x                      = 0;
            character.transform.eulerAngles = rotation;
            character.transform.localScale  = Vector3.one;
            character.transform.position    = Vector3.zero;
            character.speed                 = 1;
        }

        // This is where custom config preset is saved 
        private void GetConfigPath()
        {
#if UNITY_EDITOR
            configPath = Application.dataPath + "/CharacterCreator2D/Creator UI/Config/";
#else
            configPath = Application.dataPath + "/../Config/";
#endif
        }

        // Update the custom preset dropdown
        private void UpdateConfigDropdown()
        {
            configPreset.ClearOptions();
            configPreset.options.Add(new Dropdown.OptionData("Custom"));

            if (!Directory.Exists(configPath))
            {
                return;
            }

            var dir   = new DirectoryInfo(configPath);
            var files = dir.GetFiles("*" + configExtension);

            if (files.Length <= 0)
            {
                SetConfigToCustom();
            } else
            {
                foreach (var f in files)
                {
                    var s = f.Name.Replace(configExtension, "");
                    configPreset.options.Add(new Dropdown.OptionData(s));
                }
            }

            configPreset.RefreshShownValue();
        }

        // Set the preset to custom
        private void SetConfigToCustom()
        {
            if (isLoadingconfig)
            {
                return;
            }

            currentPreset      = 0;
            configPreset.value = 0;
        }

        // Erase / delete custom preset
        public void EraseConfig()
        {
            var configName = configPreset.options[configPreset.value].text;
            var path       = configPath + configName + configExtension;
            if (File.Exists(path))
            {
                runtimeDialog.DisplayDialog("Delete Preset", "Are you sure you want to remove '" + configName + "' preset?", true);
                runtimeDialog.yesButton.onClick.AddListener(delegate { ConfirmEraseConfig(path); });
            }
        }

        private void ConfirmEraseConfig(string path)
        {
#if UNITY_EDITOR
            var editorPath = path.Replace(Application.dataPath, "Assets");
            AssetDatabase.DeleteAsset(editorPath);
            AssetDatabase.Refresh();
#else
            File.Delete(path);
#endif
            SetConfigToCustom();
            UpdateConfigDropdown();
        }

        // Save current config into a custom preset
        public void SaveConfig(string configName)
        {
            if (string.IsNullOrEmpty(configName))
            {
                return;
            }

            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }

            var path = configPath + configName + configExtension;

            if (File.Exists(path))
            {
                runtimeDialog.DisplayDialog("Save Preset", "Preset '" + configName + "' already exist. Do you want to replace it?", true);
                runtimeDialog.yesButton.onClick.AddListener(delegate { ConfirmSaveConfig(path, configName); });
            } else
            {
                runtimeDialog.DisplayDialog("Save Preset", "Save preset as '" + configName + "'?", true);
                runtimeDialog.yesButton.onClick.AddListener(delegate { ConfirmSaveConfig(path, configName); });
            }
        }

        private void ConfirmSaveConfig(string path, string configName)
        {
            var config = new ExportConfig();

            config.targetFPS    = targetFPS;
            config.width        = width;
            config.height       = height;
            config.bgColor      = bgColor;
            config.exportMode   = exportMode;
            config.charPosition = charPosition;
            config.charScale    = charScale;
            config.scaleMode    = scaleMode;

            var file = JsonUtility.ToJson(config);
            File.WriteAllText(path, file);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            UpdateConfigDropdown();
            for (var i = 0; i < configPreset.options.Count; i++)
            {
                if (configPreset.options[i].text == configName)
                {
                    currentPreset      = i;
                    configPreset.value = i;
                    break;
                }
            }
        }

        // Load config from custom preset
        public void LoadConfig(int i)
        {
            if (i == 0)
            {
                return;
            }

            var configName = configPreset.options[i].text;
            LoadConfig(configName);
        }

        public void LoadDefaultConfig(TextAsset ta)
        {
            var config = JsonUtility.FromJson<ExportConfig>(ta.text);
            LoadConfig(config);
            SetConfigToCustom();

            if (ta.name == "Atlas")
            {
                aimAnimation.value = 0;

                for (var i = baseAnimation.options.Count; i-- > 0;)
                {
                    if (baseAnimation.options[i].text == "Atlas")
                    {
                        baseAnimation.value = i;
                        break;
                    }
                }
            } else if (ta.name == "Animation")
            {
                baseAnimation.value = 1;
                aimAnimation.value  = 0;
            } else
            {
                baseAnimation.value = 0;
                aimAnimation.value  = 0;
            }
        }

        private void LoadConfig(string configName)
        {
            if (currentPreset == configPreset.value)
            {
                return;
            }

            currentPreset = configPreset.value;
            var path = configPath + configName + configExtension;

            if (!File.Exists(path))
            {
                return;
            }

            var config = JsonUtility.FromJson<ExportConfig>(File.ReadAllText(path));
            LoadConfig(config);
        }

        private void LoadConfig(ExportConfig config)
        {
            var stringFormat = "0.00";
            var ci           = new CultureInfo("");
            ci.NumberFormat.NumberDecimalSeparator = ".";

            isLoadingconfig = true;
            inputFPS.text   = config.targetFPS.ToString();
            inputWidth.text = config.width.ToString();
            SetWidth(config.width.ToString());
            inputHeight.text = config.height.ToString();
            SetHeight(config.height.ToString());
            inputExportMode.value = (int) config.exportMode;
            inputOffsetX.text     = config.charPosition.x.ToString(stringFormat, ci);
            inputOffsetY.text     = config.charPosition.y.ToString(stringFormat, ci);
            inputScale.text       = config.charScale.x.ToString(stringFormat, ci);
            inputScaleMode.value  = (int) config.scaleMode;
            UpdateSpriteSize();
            isLoadingconfig = false;
        }

        // Set the width of the target output
        public void SetWidth(string w)
        {
            if (string.IsNullOrEmpty(w))
            {
                return;
            }

            width = int.Parse(w);
            UpdateSpriteSize();
        }

        // Set the height of the target output
        public void SetHeight(string h)
        {
            if (string.IsNullOrEmpty(h))
            {
                return;
            }

            height = int.Parse(h);
            UpdateSpriteSize();
        }

        // Set the scale mode of the character
        public void SetScaleMode(int i)
        {
            scaleMode = (ScaleMode) i;
            UpdateCameraZoom();
        }

        // Update the preview sprite size according to target resolution
        private void UpdateSpriteSize()
        {
            renderTexture            = new RenderTexture(width * superSampling, height * superSampling, 24, RenderTextureFormat.ARGB32);
            renderTexture.filterMode = FilterMode.Bilinear;
            previewCam.targetTexture = renderTexture;
            previewUI.texture        = renderTexture;
            previewUI.SetNativeSize();
            previewUIContainer.sizeDelta = previewUI.rectTransform.sizeDelta;
            UpdateCameraZoom();
        }

        // Update the zoom of the preview camera to reflect the changes made within the configuration
        private void UpdateCameraZoom()
        {
            if (scaleMode == ScaleMode.ActualPixelSize)
            {
                previewCam.orthographicSize = height / PPU;
            } else
            {
                previewCam.orthographicSize = 10;
            }

            SetConfigToCustom();
        }

        // Initialize the emote dropdown
        private void InitEmoteDropdown()
        {
            emoteDropdown.ClearOptions();
            emoteDropdown.options.Add(new Dropdown.OptionData());
            emoteDropdown.options[0].text = "None";

            var s = Enum.GetNames(typeof(EmotionType));
            for (var i = 0; i < s.Length; i++)
            {
                var name = characterViewer.emotes.getIndex((EmotionType) i).name;
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                s[i] = name;
            }

            var names = new List<string>(s);
            emoteDropdown.AddOptions(names);
        }

        // Adjust the position of the character
        public void SetCharPosX(float x)
        {
            charPosition.x = x;
            UpdateCharacter();
        }

        public void SetCharPosY(float y)
        {
            charPosition.y = y;
            UpdateCharacter();
        }

        // Adjust the scale of the character
        public void SetCharScale(float s)
        {
            charScale.x = s;
            charScale.y = s;
            UpdateCharacter();
        }

        // Flip the character on the X axis
        public void SetCharFlip(bool flip)
        {
            flipCharacter = flip;
            var rotation = Vector3.zero;
            if (flip)
            {
                rotation.y = 180;
            } else
            {
                rotation.y = 0;
            }

            character.transform.eulerAngles = rotation;
        }

        // Update the character position and scale
        private void UpdateCharacter()
        {
            character.transform.position   = charPosition;
            character.transform.localScale = charScale;
            SetConfigToCustom();
        }

        // Set the target FPS of the output
        public void SetFPS(string s)
        {
            targetFPS = int.Parse(s);
            CalculateFrameCount();
            SetConfigToCustom();
        }

        // Play and pause the preview animation
        public void PlayAnimation(bool play)
        {
            if (play)
            {
                character.speed = 1;
            } else
            {
                character.speed = 0;
            }
        }

        // Apply emote to character
        public void Emote(int i)
        {
            if (i - 1 < 0)
            {
                characterViewer.ResetEmote();
            } else
            {
                characterViewer.Emote((EmotionType) i - 1);
            }
        }

        // Set the aim angle of the character
        public void SetAimAngle(float f)
        {
            character.SetFloat("Aim", f);
        }

        // Update the zoom value of the preview window
        public void UpdateZoomPreview(float f)
        {
            f                             /= superSampling;
            previewUIContainer.localScale =  new Vector3(f, f, 1);
        }

        // Set the export mode for the output
        public void SetExportMode(int i)
        {
            exportMode = (ExportMode) i;
        }

        // Start the process of exporting to PNG
        public void Export()
        {
            if (exportMode == ExportMode.SingleImage)
            {
                SaveAsPNG();
            } else if (exportMode == ExportMode.PNGSequence)
            {
                StartCoroutine(SaveSequence());
            }
        }

        public void GetAnimationLength()
        {
            // Get target animation length in each layer
            // Base layer
            selectedAnimation = baseAnimation.options[baseAnimation.value].text;
            character.Update(0);
            var info = character.GetCurrentAnimatorClipInfo(0);
            if (0 < info.Length)
            {
                baseLength = info[0].clip.length;
            } else
            {
                baseLength = 0;
            }

            //Aim layer
            selectedAim = aimAnimation.options[aimAnimation.value].text;
            character.Update(0);
            var aimStateInfo = character.GetCurrentAnimatorClipInfo(1);
            if (0 < aimStateInfo.Length)
            {
                aimLength = aimStateInfo[0].clip.length;
            } else
            {
                aimLength = 0;
            }

            // Calculate the target length of the sequence
            // if aim animation is longer, and loop aim layer is turned off,
            // loop the base animation until aim animation is done
            targetLength = baseLength;
            if (targetLength <= 0 && aimLength > targetLength)
            {
                targetLength = aimLength;
            } else if (!loopAimLayer)
            {
                while (aimLength > targetLength)
                {
                    targetLength += baseLength;
                }
            }

            animationLengthUI.text = targetLength.ToString();

            CalculateFrameCount();
        }

        private void CalculateFrameCount()
        {
            // Calculate final frame count that will be exported
            frameCount = (int) (targetLength * targetFPS);
            if (frameCount <= 0)
            {
                frameCount = 1;
            }

            frameCountUI.text = frameCount.ToString();
        }

        private void SaveAsPNG()
        {
            // Show save file dialog
#if UNITY_EDITOR
            path = EditorUtility.SaveFilePanel("Export Single Image", "Assets/", fileName, "png");
#endif

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            // Pause character animation
            character.speed = 0;
            playToggle.isOn = false;

            // Render and save to file
            var tex2D = new Texture2D(width * superSampling, height * superSampling, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            previewCam.Render();
            tex2D.ReadPixels(new Rect(0, 0, width * superSampling, height * superSampling), 0, 0, false);
            TextureScaler.Bilinear(tex2D, width, height);
            tex2D.Apply();
            var bytes = tex2D.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            RenderTexture.active = null;
            Destroy(tex2D);

            // Show success dialog box
            Debug.Log("Saved to " + path);
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Save Successful", "Saved to " + path, "OK");
#else
            runtimeDialog.DisplayDialog("Save Successful", "PNG saved to <color=#178294>" + path + "</color> successfully.");
#endif
        }

        private IEnumerator SaveSequence()
        {
            // Show save dialog
#if UNITY_EDITOR
            path = EditorUtility.SaveFilePanel("Export Sequence", "Assets/", fileName, "png");
#endif

            if (string.IsNullOrEmpty(path))
            {
                yield break;
            }

            // Split path into base (without extension) and directory path to use when saving sequence
            var basePath = path.Replace(".png", "_");

            // Get animation length
            GetAnimationLength();
            var   frameDelay  = 1 / (float) targetFPS;
            float elapsedTime = 0;

            // Pause character animation and reset aim layer to make sure muzzle flash particle is off
            playToggle.isOn = false;
            character.Play("None", 1, 0);
            yield return null;

            // Iterate and render through each frame of the sequence
            for (var i = 0; i < frameCount; i++)
            {
                yield return new WaitForSeconds(frameDelay);

                // update progress bar
                var progress = ((float) i + 1) / frameCount;
#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar("Export Sequence", "Exporting frame " + i + "/" + frameCount, progress);
#else
                runtimeDialog.DisplayProgressBar("Export Sequence", "Exporting frame " + i.ToString() + "/" + frameCount.ToString(), progress);
#endif

                // Set the character animation and convert animation length into normalized time				
                var baseTime = i * frameDelay / baseLength;
                character.Play(selectedAnimation, 0, baseTime);
                character.speed = 0;
                // Only play aim layer once and clamp the animation at the end
                // or keep looping if loop aim layer is set to true
                if (elapsedTime <= aimLength || loopAimLayer)
                {
                    var aimTIme = i * frameDelay / aimLength;
                    character.Play(selectedAim, 1, aimTIme);
                    character.speed = 0;
                }

                // Wait til animator is updated properly
                elapsedTime += frameDelay;
                yield return null;

                // Render and save the frame
                var tex2D = new Texture2D(width * superSampling, height * superSampling, TextureFormat.RGBA32, false);
                RenderTexture.active = renderTexture;
                previewCam.Render();
                tex2D.ReadPixels(new Rect(0, 0, width * superSampling, height * superSampling), 0, 0, false);
                TextureScaler.Bilinear(tex2D, width, height);
                tex2D.Apply();
                var bytes = tex2D.EncodeToPNG();

                path = basePath + i.ToString("000") + ".png";
                File.WriteAllBytes(path, bytes);

                RenderTexture.active = null;
                Destroy(tex2D);
            }

            // Show success dialog box
            Debug.Log("PNG sequences saved to " + path);
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Save Successful", "PNG sequence saved to " + path, "OK");
#else
            runtimeDialog.DisplayDialog("Save Successful", "PNG sequence saved to <color=#178294>" + path + "</color> successfully.");
#endif
        }
    }

    public enum ExportMode
    {
        SingleImage,
        PNGSequence
    }

    public enum ScaleMode
    {
        Automatic,
        ActualPixelSize
    }

    public enum BackgroundColor
    {
        Transparent,
        Black,
        White,
        Gray,
        Magenta,
        Green
    }

    public class ExportConfig
    {
        public int             targetFPS    = 12;
        public int             width        = 512;
        public int             height       = 512;
        public BackgroundColor bgColor      = 0;
        public ExportMode      exportMode   = 0;
        public Vector3         charPosition = Vector3.zero;
        public Vector3         charScale    = Vector3.one;
        public ScaleMode       scaleMode    = 0;
    }
}