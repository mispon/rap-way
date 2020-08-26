using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utils.Carousel
{
    public enum CarouselMode
    {
        All,
        TextOnly,
        ImageOnly
    }
    
    /// <summary>
    /// Элемент карусели
    /// </summary>
    public class CarouselItem : MonoBehaviour
    {
        [SerializeField] private CarouselMode mode;
        [SerializeField] private Button button;
        [Space]
        [SerializeField] private Text label;
        [SerializeField] private Image image;

        private object _value;

        /// <summary>
        /// Возвращает значение элемента 
        /// </summary>
        public T GetValue<T>() => (T) _value;

        /// <summary>
        /// Инициализация элемента карусели
        /// </summary>
        public void Setup(CarouselProps props, Action clickCallback)
        {
            if (mode == CarouselMode.All)
                SetAll(props);
            if (mode == CarouselMode.TextOnly)
                SetText(props.Text);
            if (mode == CarouselMode.ImageOnly)
                SetImage(props.Sprite);
            
            _value = props.Value;
            button.onClick.AddListener(clickCallback.Invoke);
            
            gameObject.SetActive(true);
        }

        private void SetAll(CarouselProps props)
        {
            SetText(props.Text);
            SetImage(props.Sprite);
        }

        private void SetText(string text) => label.text = text;
        
        private void SetImage(Sprite sprite) => image.sprite = sprite;
    }

    [Serializable]
    public class CarouselProps
    {
        public string Text;
        public Sprite Sprite;
     
        [NonSerialized] public object Value;
    }
}