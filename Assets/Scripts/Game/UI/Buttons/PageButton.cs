using Core;
using Game.Pages;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Buttons
{
    public enum PageActionType
    {
        Open, Close
    }
    
    /// <summary>
    /// Логика кнопок управления состоянием страницы
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class PageButton : MonoBehaviour
    {
        [Header("НАстройки страницы")]
        [SerializeField] protected Page page;
        [SerializeField] protected PageActionType action;

        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            SoundManager.Instance.PlayClick();
            PageAction();
        }

        protected virtual void PageAction()
        {
            if (action == PageActionType.Open)
                page.Open();
            else
                page.Close();
        }
    }
}