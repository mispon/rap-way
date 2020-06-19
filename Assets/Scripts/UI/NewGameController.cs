using System;
using System.Linq;
using Core;
using Enums;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class NewGameController: MonoBehaviour
    {
        [Header("Поля данных")] 
        [SerializeField] private InputField[] inputFields;
        [SerializeField] private Text DateOfBirth;
        [SerializeField] private Dropdown Race;
        [SerializeField] private Toggle[] Genders;

        [Header("Вспомогательные данные")] 
        ///X: левая граница Y: правая граница Z: текущее значение 
        [SerializeField] private Vector3Int ageControlValues;

        private void Start()
        {
            //Заполнение списка Рас
            Race.AddOptions(
                Enum.GetNames(typeof(Race))
                    .Select(item => new Dropdown.OptionData(item))
                    .ToList());
            Race.value = 0;

            SetDateOfBirthValue();
        }

        #region Age Control
        public void DecreaseAge()
        {
            if(ageControlValues.z == ageControlValues.x)
                return;

            ageControlValues.z--;
            SetDateOfBirthValue();
        }

        public void IncreaseAge()
        {
            if(ageControlValues.z == ageControlValues.y)
                return;
            
            ageControlValues.z++;
            SetDateOfBirthValue();
        }
        private void SetDateOfBirthValue()
        {
            DateOfBirth.text = ageControlValues.z.ToString();
        }
        #endregion

        #region Gender control

        public void OnGenderValueChange(int index)
        {
            Genders[(index + 1) % 2].isOn = !Genders[index].isOn;
        }
        

        #endregion
        
        
        #region Validation
        public void Validate()
        {
            //Поля ввода
            foreach (var field in inputFields)
                if(!CheckNotNullOrEmpty(field))
                    return;
            
            //Возраст валиден
            //Раса всегда выбрана
            
            //Пол
            if(!CheckToggles())
                return;
            
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

        private bool CheckToggles()
        {
            if (Genders[0].isOn == Genders[1].isOn)
            {
                foreach (var tog in Genders)
                    HighlightError(tog);
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

        public void Confirm()
        {

            PlayerInfo newPlayer = new PlayerInfo
            {
                FirstName = inputFields[0].text,
                LastName = inputFields[1].text,
                NickName = inputFields[2].text,
                HomeLand = inputFields[3].text,

                CreationDate = DateTime.Now,
                DateOfBirth = DateTime.Today - TimeSpan.FromDays(365 * Convert.ToInt32(DateOfBirth.text)),

                Race = (Race) Enum.Parse(typeof(Race), Race.options[Race.value].text),

                Gender = Genders[0].isOn ? Gender.Male : Gender.Female
            };
            
            Debug.Log($"Здорова {newPlayer}");
            //ToDo: пока не сохраняю в GamaManager.PlayerData.Info
            Debug.Log("Я тебя не сохранил");
        }
    }
}