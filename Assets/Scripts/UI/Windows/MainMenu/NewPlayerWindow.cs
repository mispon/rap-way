using System;
using System.Linq;
using Core;
using Enums;
// using Firebase.Analytics;
using Game;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using ScriptableObjects;
using UI.Base;
using UI.Controls.Carousel;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class NewPlayerWindow : CanvasUIElement
    {
        [Header("Sprites")]
        [SerializeField] private Sprite _maleAvatar;
        [SerializeField] private Sprite _femaleAvatar;
        [SerializeField] private Sprite _maleAvatarInactive;
        [SerializeField] private Sprite _femaleAvatarInactive;
        
        [Header("Data fields")]
        [SerializeField] private Button _maleButton;
        [SerializeField] private Button _femaleButton;
        [SerializeField] private InputField[] _inputFields;
        [SerializeField] private Carousel _ageCarousel;

        [Header("Buttons")]
        [SerializeField] private Button _startButton;

        private bool _maleSelected = true;

        public override void Initialize()
        {
            _maleButton.onClick.AddListener(() => OnGenderChange(true));
            _femaleButton.onClick.AddListener(() => OnGenderChange(false));
            _startButton.onClick.AddListener(OnStartClick);
        }

        private void OnGenderChange(bool isMale)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            _maleSelected = isMale;
            _maleButton.image.sprite = isMale ? _maleAvatar : _maleAvatarInactive;
            _femaleButton.image.sprite = !isMale ? _femaleAvatar : _femaleAvatarInactive;
        }
        
        private void OnStartClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            if (_inputFields.Any(field => !CheckNotNullOrEmpty(field)))
                return;
            
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
        
        /// <summary>
        /// Show errors
        /// </summary>
        private static void HighlightError(Component component)
        {
            var errorAnim = component.GetComponentInChildren<Animation>();
            errorAnim.Play();
        }

        /// <summary>
        /// Creates new character
        /// </summary>
        private void CreatePlayer()
        {
            var player = GameManager.Instance.CreateNewPlayer().Info;

            player.Gender = _maleSelected ? Gender.Male : Gender.Female;
            player.FirstName = _inputFields[0].text.Trim();
            player.LastName  = _inputFields[1].text.Trim();
            player.NickName  = _inputFields[2].text.Trim();
            player.Age = Convert.ToInt32(_ageCarousel.GetLabel());

            GameManager.Instance.SaveApplicationData();
            
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewGameStart);
            SceneMessageBroker.Instance.Publish(new SceneLoadMessage
            {
                SceneType = SceneType.Game
            });
        }
    }
}