using System;
using Core;
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
        private void Init() => Init(props);

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
        /// Возвращает текущий индекс
        /// </summary>
        public int Index => _index;

        /// <summary>
        /// Возвращает значение активного элемента 
        /// </summary>
        public T GetValue<T>() => _items[_index].GetValue<T>();

        /// <summary>
        /// Возвращает текстовое значение элемента 
        /// </summary>
        public string GetLabel() => _items[_index].GetLabel();

        /// <summary>
        /// Напрямую устанавливает новый индекс
        /// </summary>
        public void SetIndex(int index)
        {
            _index = index;
            OnArrowClicked(0, silent: true);
        }
        
        /// <summary>
        /// Напрямую устанавливает новый индекс по текстовому значению
        /// </summary>
        public void SetIndex(string label)
        {
            int index = 0;
            foreach (var item in _items)
            {
                if (string.Equals(label, item.GetLabel(), StringComparison.InvariantCultureIgnoreCase))
                {
                    SetIndex(index);
                    break;
                }

                index++;
            }
        }

        /// <summary>
        /// Очищает карусель
        /// </summary>
        private void Clear()
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
            SoundManager.Instance.PlayClick();
            onClick.Invoke(_index);
        }

        /// <summary>
        /// Обработчик переключения элемента 
        /// </summary>
        private void OnArrowClicked(int direction, bool silent = false)
        {
            if (!silent)
                SoundManager.Instance.PlaySwitch();
            
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