using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CharacterCreator2D;
using CharacterCreator2D.UI;
using Core;
using Core.Analytics;
using Enums;
using Game;
using Game.Player.Character;
using MessageBroker;
using MessageBroker.Messages.UI;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using ScriptableObjects;
using UI.Base;
using UI.Controls.Carousel;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu.NewGame
{
    public class PlayerInfoWindow : CanvasUIElement
    {
        [Header("Preview")]
        [SerializeField] private Camera previewCamera;
        [SerializeField] private RawImage preview;

        [Header("Data fields")]
        [SerializeField] private InputField[] _inputFields;
        [SerializeField] private Carousel _ageCarousel;

        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button backButton;

        private RenderTexture _renderTexture;

        public override void Initialize()
        {
            startButton.onClick.AddListener(OnStartClick);
            backButton.onClick.AddListener(() =>
            {
                MsgBroker.Instance.Publish(new WindowControlMessage
                {
                    Type = WindowType.CharacterCreator,
                    Context = new Dictionary<string, object>
                    {
                        ["dont_reset"] = true
                    }
                });
            });
        }

        protected override void BeforeShow(object ctx = null)
        {
            Character.Instance.SetAnimationSpeed(0);
            previewCamera.gameObject.SetActive(true);

            SetupPreview();
        }

        protected override void AfterHide()
        {
            Character.Instance.SetAnimationSpeed(1);
            previewCamera.gameObject.SetActive(false);
        }

        private void SetupPreview()
        {
            var width  = Convert.ToInt32(preview.rectTransform.rect.width);
            var height = Convert.ToInt32(preview.rectTransform.rect.height);

            _renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Bilinear
            };

            previewCamera.targetTexture = _renderTexture;
            preview.texture             = _renderTexture;
        }

        private void OnStartClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            if (_inputFields.Any(field => !CheckNotNullOrEmpty(field)))
            {
                return;
            }

            CreatePlayer();
            SavePortrait();

            AnalyticsManager.LogEvent(FirebaseGameEvents.NewGameStart);
            SceneMessageBroker.Instance.Publish(new SceneLoadMessage
            {
                SceneType = SceneType.Game
            });
        }

        private static bool CheckNotNullOrEmpty(InputField field)
        {
            if (string.IsNullOrWhiteSpace(field.text) || field.text.Length < 2 || field.text.Length > 20)
            {
                HighlightError(field);
                return false;
            }

            return true;
        }

        private static void HighlightError(Component component)
        {
            var errorAnim = component.GetComponentInChildren<Animation>();
            errorAnim.Play();
        }

        private void CreatePlayer()
        {
            var player = GameManager.Instance.CreateNewPlayer().Info;

            player.FirstName = _inputFields[0].text.Trim();
            player.LastName  = _inputFields[1].text.Trim();
            player.NickName  = _inputFields[2].text.Trim();
            player.Age       = Convert.ToInt32(_ageCarousel.GetLabel());

            player.Gender = Character.Instance.Viewer.bodyType == BodyType.Male
                ? Gender.Male
                : Gender.Female;

            GameManager.Instance.SaveApplicationData();
            Character.Instance.Save();
        }

        private void SavePortrait()
        {
            var width  = Convert.ToInt32(preview.rectTransform.rect.width);
            var height = Convert.ToInt32(preview.rectTransform.rect.height);
            var tex2D  = new Texture2D(width, height, TextureFormat.RGBA32, false);

            RenderTexture.active = _renderTexture;
            previewCamera.Render();

            tex2D.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
            TextureScaler.Bilinear(tex2D, width, height);
            tex2D.Apply();

            var player   = GameManager.Instance.PlayerData.Info;
            var filename = $"{player.NickName.ToLower()}.png";
            var path     = Path.Combine(Application.persistentDataPath, "Portraits");

            var bytes = tex2D.EncodeToPNG();
            File.WriteAllBytes($"{path}/{filename}", bytes);

            var sprite = Sprite.Create(tex2D, new Rect(0.0f, 0.0f, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100.0f);
            sprite.name = filename;
            SpritesManager.Instance.AppendPortrait(sprite);
        }
    }
}