using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Carousel
{
    /// <summary>
    /// Слайдер карусель 
    /// </summary>
    public class Carousel : MonoBehaviour
    {
        [Header("Стрелочки переключения элементов")]
        [SerializeField] private Button leftArrow;
        [SerializeField] private Button rightArrow;

        [Header("Настройки карусели")]
        [SerializeField] private Transform itemsContainer;
        [SerializeField] private CarouselItem itemPrefab;
        [SerializeField] private CarouselProps[] props;
        [SerializeField] private bool onAwake;
        
        public event Action<int> onChange = index => {}; 
        public event Action<int> onClick = index => {};

        private int _index;
        private CarouselItem[] _items;

        private void Awake()
        {
            leftArrow.onClick.AddListener(() => OnArrowClicked(-1));
            rightArrow.onClick.AddListener(() => OnArrowClicked(1));
            
            if (onAwake) Init();
        }

        /// <summary>
        /// Инициализация карусели
        /// </summary>
        public void Init() => Init(props);

        /// <summary>
        /// Инициализация карусели
        /// </summary>
        public void Init(CarouselProps[] itemProps)
        {
            Clear();

            _index = 0;
            _items = new CarouselItem[itemProps.Length];
            
            for (var i = 0; i < itemProps.Length; i++)
            {
                var item = Instantiate(itemPrefab, itemsContainer);
                item.Setup(itemProps[i], onElementClicked);
                item.name = $"Item-{i}";
                
                _items[i] = item;
            }
            
            UpdateItems();
        }

        /// <summary>
        /// Возвращает значение активного элемента 
        /// </summary>
        public T GetValue<T>() => _items[_index].GetValue<T>();

        /// <summary>
        /// Очищает карусель
        /// </summary>
        public void Clear()
        {
            if (_items == null)
                return;
            
            foreach (var item in _items)
                Destroy(item.gameObject);
        }

        /// <summary>
        /// Обработчик нажатия по элементу
        /// </summary>
        private void onElementClicked()
        {
            onClick.Invoke(_index);
        }

        /// <summary>
        /// Обработчик переключения элемента 
        /// </summary>
        private void OnArrowClicked(int direction)
        {
            _index += direction;
            ClampIndex();
            UpdateItems();
            onChange.Invoke(_index);
        }

        /// <summary>
        /// Обрабатывает граничные значения индекса
        /// </summary>
        private void ClampIndex()
        {
            if (_index < 0)
                _index = _items.Length - 1;
            
            if (_index == _items.Length)
                _index = 0;
        }

        /// <summary>
        /// Обновляет состояние элементов
        /// </summary>
        private void UpdateItems()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                _items[i].gameObject.SetActive(i == _index);
            }
        }
    }
} 
