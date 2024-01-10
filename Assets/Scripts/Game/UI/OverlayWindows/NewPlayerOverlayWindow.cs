using System;
using System.Linq;
using Core;
using Data;
using Enums;
using Firebase.Analytics;
using Scenes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utils.Carousel;

namespace Game.UI.OverlayWindows
{
    public class NewPlayerOverlayWindow : CanvasUIElement
    {
        [BoxGroup("Sprites")] [SerializeField] private Sprite _maleAvatar;
        [BoxGroup("Sprites")] [SerializeField] private Sprite _femaleAvatar;
        [BoxGroup("Sprites")] [SerializeField] private Sprite _maleAvatarInactive;
        [BoxGroup("Sprites")] [SerializeField] private Sprite _femaleAvatarInactive;
        
        [BoxGroup("Data fields")] [SerializeField] private Button _maleButton;
        [BoxGroup("Data fields")] [SerializeField] private Button _femaleButton;
        [BoxGroup("Data fields")] [SerializeField] private InputField[] _inputFields;
        [BoxGroup("Data fields")] [SerializeField] private Carousel _ageCarousel;

        [BoxGroup("Buttons")] [SerializeField] private Button _startButton;

        private bool _maleSelected = true;
        
        protected override void SetupListenersOnInitialize()
        {
            base.SetupListenersOnInitialize();
            
            _maleButton.onClick.AddListener(() => OnGenderChange(true));
            _femaleButton.onClick.AddListener(() => OnGenderChange(false));
            _startButton.onClick.AddListener(OnStartClick);
        }

        /// <summary>
        /// Обработчик изменения пола персонажа 
        /// </summary>
        private void OnGenderChange(bool isMale)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            _maleSelected = isMale;
            _maleButton.image.sprite = isMale ? _maleAvatar : _maleAvatarInactive;
            _femaleButton.image.sprite = !isMale ? _femaleAvatar : _femaleAvatarInactive;
        }

        /// <summary>
        /// Обработчик кнопки создания персонажа
        /// </summary>
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
        /// Подсвечивает невалидное поле 
        /// </summary>
        private static void HighlightError(Component component)
        {
            var errorAnim = component.GetComponentInChildren<Animation>();
            errorAnim.Play();
        }

        /// <summary>
        /// Cоздание нового персонажа
        /// </summary>
        private void CreatePlayer()
        {
            var player = GameManager.Instance.CreateNewPlayer().Info;

            player.Gender = _maleSelected ? Gender.Male : Gender.Female;
            player.FirstName = _inputFields[0].text.Trim();
            player.LastName  = _inputFields[1].text.Trim();
            player.NickName  = _inputFields[2].text.Trim();
            player.Age = Convert.ToInt32(_ageCarousel.GetLabel());

            FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewGameStart);
            ScenesController.Instance.MessageBroker
                .Publish(new SceneLoadMessage()
                {
                    sceneType = SceneTypes.Game
                });
        }
    }
}