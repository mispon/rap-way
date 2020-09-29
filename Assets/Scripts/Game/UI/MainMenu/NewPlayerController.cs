using System;
using System.Linq;
using Core;
using Enums;
using UnityEngine;
using UnityEngine.UI;
using Utils.Carousel;

namespace Game.UI.MainMenu
{
    /// <summary>
    /// Окно создания нового персонажа
    /// </summary>
    public class NewPlayerController: MonoBehaviour {
        [Header("Картинки")]
        [SerializeField] private Sprite maleAvatar;
        [SerializeField] private Sprite femaleAvatar;
        [SerializeField] private Sprite maleAvatarInactive;
        [SerializeField] private Sprite femaleAvatarInactive;
        
        [Header("Поля данных")]
        [SerializeField] private Button maleButton;
        [SerializeField] private Button femaleButton;
        [SerializeField] private InputField[] inputFields;
        [SerializeField] private Carousel ageCarousel;
        [Space]
        [SerializeField] private Button startButton;
        [SerializeField] private Button exitButton;

        private bool _maleSelected = true;
        
        private void Start()
        {
            maleButton.onClick.AddListener(() => OnGenderChange(true));
            femaleButton.onClick.AddListener(() => OnGenderChange(false));
            startButton.onClick.AddListener(OnStartClick);
            exitButton.onClick.AddListener(()=> MainMenuController.SetWindowActive(gameObject, false));
        }

        /// <summary>
        /// Обработчик изменения пола персонажа 
        /// </summary>
        private void OnGenderChange(bool isMale)
        {
            _maleSelected = isMale;
            maleButton.image.sprite = isMale ? maleAvatar : maleAvatarInactive;
            femaleButton.image.sprite = !isMale ? femaleAvatar : femaleAvatarInactive;
        }

        /// <summary>
        /// Обработчик кнопки создания персонажа
        /// </summary>
        private void OnStartClick()
        {
            if (inputFields.Any(field => !CheckNotNullOrEmpty(field)))
                return;

            CreatePlayer();
        }
        
        private static bool CheckNotNullOrEmpty(InputField field)
        {
            if (!string.IsNullOrEmpty(field.text))
                return true;
            
            HighlightError(field);
            return false;
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
            var player = GameManager.Instance.PlayerData.Info;

            player.Gender = _maleSelected ? Gender.Male : Gender.Female;
            player.FirstName = inputFields[0].text;
            player.LastName  = inputFields[1].text;
            player.NickName  = inputFields[2].text;
            player.Age = Convert.ToInt32(ageCarousel.GetLabel());

            player.CreationDate = DateTime.Now;

            SceneManager.Instance.LoadGameScene();
        }
    }
}