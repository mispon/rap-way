using System;
using System.Linq;
using CharacterCreator2D;
using Core;
using Core.Analytics;
using Enums;
using Game;
using Game.Player.Character;
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
        [Header("Sprites")]
        [SerializeField] private Sprite _maleAvatar;
        [SerializeField] private Sprite _femaleAvatar;
        [SerializeField] private Sprite _maleAvatarInactive;
        [SerializeField] private Sprite _femaleAvatarInactive;

        [Header("Data fields")]
        [SerializeField] private InputField[] _inputFields;
        [SerializeField] private Carousel _ageCarousel;

        [Header("Buttons")]
        [SerializeField] private Button _startButton;

        public override void Initialize()
        {
            _startButton.onClick.AddListener(OnStartClick);
        }

        private void OnStartClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            if (_inputFields.Any(field => !CheckNotNullOrEmpty(field)))
            {
                return;
            }

            CreatePlayer();
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

            AnalyticsManager.LogEvent(FirebaseGameEvents.NewGameStart);
            SceneMessageBroker.Instance.Publish(new SceneLoadMessage
            {
                SceneType = SceneType.Game
            });
        }
    }
}