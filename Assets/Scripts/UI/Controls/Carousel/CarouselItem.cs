using System;
using Core.Events;
using Core.Localization;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace UI.Controls.Carousel
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
        private string _text;
        private bool _localized;

        private void Start()
        {
            EventManager.AddHandler(EventType.LangChanged, OnLandChanged);
        }

        /// <summary>
        /// Возвращает значение элемента 
        /// </summary>
        public T GetValue<T>() => (T) _value;

        /// <summary>
        /// Возвращает текстовое обозначение элемента 
        /// </summary>
        public string GetLabel() => label.text;

        /// <summary>
        /// Инициализация элемента карусели
        /// </summary>
        public void Setup(CarouselProps props, Action clickCallback)
        {
            if (mode == CarouselMode.All)
                SetAll(props);
            if (mode == CarouselMode.TextOnly)
                SetText(props.Text, props.Localized);
            if (mode == CarouselMode.ImageOnly)
                SetImage(props.Sprite);
            
            _value = props.Value;
            button.onClick.AddListener(clickCallback.Invoke);

            DisplayLabel();
            gameObject.SetActive(true);
        }

        private void SetAll(CarouselProps props)
        {
            SetText(props.Text, props.Localized);
            SetImage(props.Sprite);
        }

        private void SetText(string text, bool localized)
        {
            _text = text;
            _localized = localized;
        }
        
        private void SetImage(Sprite sprite) => image.sprite = sprite;

        /// <summary>
        /// Отображает локализованное значение
        /// </summary>
        private void DisplayLabel() =>
            label.text = _localized ? LocalizationManager.Instance.Get(_text) : _text;
        
        /// <summary>
        /// Обрабатывает смену языка 
        /// </summary>
        private void OnLandChanged(object[] args)
        {
            DisplayLabel();
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.LangChanged, OnLandChanged);
        }
    }

    [Serializable]
    public class CarouselProps
    {
        public bool Localized;
        public string Text;
        public Sprite Sprite;
     
        [NonSerialized] public object Value;
    }
}