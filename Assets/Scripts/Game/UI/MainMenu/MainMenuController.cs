using Core;
using UnityEngine;
using Utils;

namespace Game.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        private const int NEW_GAME = 0;
        private const int CONTINUE_GAME = 1;
        private const int ABOUT = 2;
        
        [SerializeField] private GameObject newPlayerScreen;
        [SerializeField] private Switcher menuSwitcher;

        private void Start()
        {
            menuSwitcher.InstantiateElements(new[] {"New Game", "Continue", "About"});
            menuSwitcher.AddClickCallback(OnItemSelect);
        }

        private void OnItemSelect(int index)
        {
            switch (index)
            {
                case NEW_GAME:
                    newPlayerScreen.SetActive(true);
                    break;
                case CONTINUE_GAME:
                    SceneManager.Instance.LoadGameScene();
                    break;
                case ABOUT:
                    
                    break;
                default:
                    throw new RapWayException("[StartMenuController] Действие не определено!");
            }
            gameObject.SetActive(false);
        }
    }    
}

