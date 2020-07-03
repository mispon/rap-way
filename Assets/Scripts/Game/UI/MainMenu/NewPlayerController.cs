using System;
using System.Linq;
using Core;
using Enums;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI.MainMenu
{
    /// <summary>
    /// Окно создания нового персонажа
    /// </summary>
    public class NewPlayerController: MonoBehaviour
    {
        [Header("Поля данных")] 
        [SerializeField] private InputField[] inputFields;
        [SerializeField] private Switcher dateOfBirthSwitcher;
        [SerializeField] private Switcher raceSwitcher;
        [SerializeField] private Switcher genderSwitcher;
        [Space]
        [SerializeField] private Button continueButton;

        private void Start()
        {
            // todo: обрабатывать локализацию значений
            dateOfBirthSwitcher.InstantiateElements(Enumerable.Range(16, 15));
            raceSwitcher.InstantiateElements(Enum.GetNames(typeof(Race)));
            genderSwitcher.InstantiateElements(Enum.GetNames(typeof(Gender)));
            
            continueButton.onClick.AddListener(OnCreateClick);
        }

        /// <summary>
        /// Обработчик кнопки создания персонажа
        /// </summary>
        private void OnCreateClick()
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

            player.FirstName = inputFields[0].text;
            player.LastName = inputFields[1].text;
            player.NickName = inputFields[2].text;
            player.HomeLand = inputFields[3].text;
            
            player.CreationDate = DateTime.Now;
            player.CreationDate = DateTime.Now;
            
            player.Race = (Race) raceSwitcher.ActiveIndex;
            player.Gender = (Gender) genderSwitcher.ActiveIndex;

            var days = TimeSpan.FromDays(365 * Convert.ToInt32(dateOfBirthSwitcher.ActiveTextValue));
            player.DateOfBirth = DateTime.Today - days;

            SceneManager.Instance.LoadGameScene();
        }
    }
}