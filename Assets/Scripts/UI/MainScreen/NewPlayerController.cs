using System;
using System.Linq;
using Enums;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.MainScreen
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
            dateOfBirthSwitcher.InstantiateElements(Enumerable.Range(16, 15));
            raceSwitcher.InstantiateElements(Enum.GetNames(typeof(Race)));
            genderSwitcher.InstantiateElements(Enum.GetNames(typeof(Gender)));
            
            continueButton.onClick.AddListener(Validate);
        }

        private void Validate()
        {
            if (inputFields.Any(field => !CheckNotNullOrEmpty(field)))
                return;

            Confirm();
        }
        
        private static bool CheckNotNullOrEmpty(InputField field)
        {
            if (!string.IsNullOrEmpty(field.text))
                return true;
            
            HighlightError(field);
            return false;
        }
        
        private static void HighlightError(Component component)
        {
            var errorAnim = component.GetComponentInChildren<Animation>();
            errorAnim.Play();
        }

        private void Confirm()
        {
            var newPlayer = new PlayerInfo
            {
                FirstName = inputFields[0].text,
                LastName = inputFields[1].text,
                NickName = inputFields[2].text,
                HomeLand = inputFields[3].text,

                CreationDate = DateTime.Now,
                DateOfBirth = DateTime.Today - TimeSpan.FromDays(365 * Convert.ToInt32(dateOfBirthSwitcher.ActiveTextValue)),

                Race = (Race)raceSwitcher.ActiveIndex,
                Gender = (Gender) genderSwitcher.ActiveIndex
            };
            
            Debug.Log($"Здорова {newPlayer}");
            //ToDo: пока не сохраняю в GamaManager.PlayerData.Info
            Debug.Log("Я тебя не сохранил");
        }
    }
}