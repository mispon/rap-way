using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Switcher menuSwitcher;

        private void Start()
        {
            //Пример инициализации значений
//            menuSwitcher.InstantiateElements(new string[]
//            {
//                "New Game",
//                "Continue",
//                "About"
//            });
            menuSwitcher.AddClickCallback(OnItemSelect);
        }

        private void OnItemSelect(int index)
        {
            Debug.Log($"Selected index: {index}, with string value: {menuSwitcher.ActiveTextValue}");
        }
    }    
}

