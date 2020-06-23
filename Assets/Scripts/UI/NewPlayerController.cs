using System;
using System.Linq;
using Enums;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.MainMenu
{
    public class NewPlayerController: MonoBehaviour
    {
        [Header("Поля данных")] 
        [SerializeField] private InputField[] inputFields;
        [SerializeField] private Switcher dateOfBirthSwitcher;
        [SerializeField] private Switcher raceSwitcher;
        [SerializeField] private Switcher genderSwitcher;

        private void Start()
        {
            dateOfBirthSwitcher.InstantiateElements(Enumerable.Range(12, 100));
            raceSwitcher.InstantiateElements(Enum.GetNames(typeof(Race)));
            genderSwitcher.InstantiateElements(Enum.GetNames(typeof(Gender)));
        }
        
        #region Validation
        public void Validate()
        {
            //Поля ввода
            foreach (var field in inputFields)
                if(!CheckNotNullOrEmpty(field))
                    return;
            
            //Возраст валиден
            //Раса всегда выбрана
            //Пол всегда выбран
            
            Confirm();
        }
        private bool CheckNotNullOrEmpty(InputField field)
        {
            if (string.IsNullOrEmpty(field.text))
            {
                HighlightError(field);
                return false;
            }
            return true;
        }
        private void HighlightError(Component component)
        {
            foreach (var anim in component.GetComponentsInChildren<Animation>())
                anim.Play();
        }
        #endregion

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

                Race = (Race) raceSwitcher.ActiveIndex,
                Gender = (Gender) genderSwitcher.ActiveIndex
            };
            
            Debug.Log($"Здорова {newPlayer}");
            //ToDo: пока не сохраняю в GamaManager.PlayerData.Info
            Debug.Log("Я тебя не сохранил");
        }
    }
}