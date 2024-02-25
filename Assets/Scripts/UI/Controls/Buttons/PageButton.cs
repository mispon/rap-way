using Core;
using ScriptableObjects;
using UI.Windows.Pages;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controls.Buttons
{
    public enum PageActionType
    {
        Open, 
        Close
    }
    
    [RequireComponent(typeof(Button))]
    public class PageButton : MonoBehaviour
    {
        [Header("Настройки страницы")]
        [SerializeField] protected Page page;
        [SerializeField] protected PageActionType action;
        [SerializeField] private bool playSound = true;
        
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (playSound)
            {
                SoundManager.Instance.PlaySound(UIActionType.Click);
            }
            PageAction();
        }

        protected virtual void PageAction()
        {
            if (action == PageActionType.Open)
            {
                page.Open();
            }
            else
            {
                page.Close();
            }
        }
    }
}