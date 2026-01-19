using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RapWay.UI.Windows
{
    public enum WindowType
    {
        HUD,
        Screen,
        Popup
    }
    
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public abstract class BaseWindow : MonoBehaviour
    {
        [SerializeField] private WindowType type;
        [SerializeField] private bool destroyOnClose;
        
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        public WindowType Type => type;
        
        protected virtual void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        public virtual async UniTask Show(object payload = null)
        {
            gameObject.SetActive(true);
            
            // TODO: DOTween animation
            _canvasGroup.alpha = 1; 
            
            OnShown(payload);
            await UniTask.CompletedTask;
        }
        
        public virtual async UniTask Hide()
        {
            // TODO: DOTween animation
            _canvasGroup.alpha = 0;
            
            gameObject.SetActive(false);

            if (destroyOnClose)
            {
                Destroy(gameObject);
            }
            
            await UniTask.CompletedTask;
        }
        
        /// <summary>
        /// For overrides in concrete child windows
        /// </summary>
        protected virtual void OnShown(object payload) { }
        
        public void SetSortingOrder(int order)
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }
            
            _canvas.sortingOrder = order;
        }
    }   
}