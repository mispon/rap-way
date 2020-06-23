using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using dd = UnityEngine.UI.Dropdown;

namespace Utils
{
    public class Switcher : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private System.Action<int> onClickAction = delegate { };
        
        [SerializeField, Tooltip("Сгенировать элементы при старте?")] 
        private bool createOnAwake;
        [SerializeField, Tooltip("Создать подписчик на старте?")] 
        private bool subscribeOnAwake;
    
        [Header("Settings")]
        [SerializeField, Tooltip("Контейнер элементов")] 
        private RectTransform container;
        [SerializeField, Tooltip("Базовый элемент")] 
        private GameObject baseElement;
        [SerializeField, Range(0.1f, 1f), Tooltip("Расстояние между элементами, как доля от их размера")] 
        private float elementSpaceWidthPercentage = 0.33f;
    
        /// <summary>
        /// Размер окна отображения
        /// </summary>
        private Vector2 _viewPortSize;
        /// <summary>
        /// Размер контейнера с элементами
        /// </summary>
        private float _containerWidth;
        private float _containerPositionPercentage;
        /// <summary>
        /// Доля текущей позции от размера контейнера
        /// </summary>
        private float containerPositionPercentage {
            get => _containerPositionPercentage;
            set => _containerPositionPercentage = value < 0 ? 0 : value > 1 ? 1 : value;
        }
        /// <summary>
        /// Правая граница доли позиции от ширины контейнера на каждый из элементов
        /// </summary>
        private float[] _percentageBoardsOnElement;
    
        [Header("Elements info")]
        [SerializeField, Tooltip("Список опций выбора")] private dd.OptionDataList listElements;
        private GameObject[] _instantiatedElements;
        public int ElementsCount => listElements.options.Count;
        /// <summary>
        /// Индекс элемента, на котором сфокисоровано окно выбора
        /// </summary>
        public int ActiveIndex { get; private set; }
        /// <summary>
        /// Объект-элемент, на котором сфокусировано окно выбора. Нахуй - не придумал
        /// </summary>
        public GameObject ActiveObject => _instantiatedElements[ActiveIndex];
        /// <summary>
        /// Опция-элемент, на котором сфокусировано окно выбора
        /// </summary>
        public dd.OptionData ActiveOption => listElements.options[ActiveIndex];
        /// <summary>
        /// Текст элемента, на котором сфокусировано окно выбора
        /// </summary>
        public string ActiveTextValue => ActiveOption.text;
    
        [Header("Scroll info")] 
        [SerializeField, Range(0.1f, 1.0f), Tooltip("Время слайда контейнера при фокусировании")] 
        private float t_Slide = 0.2f;
        [SerializeField, Range(0.02f, 1.0f), Tooltip("Время инерции при скролле")] 
        private float t_Inertia = 0.075f;
    
        [SerializeField, Tooltip("Коэффициент изменения позиции мыши")]
        private float dragMultiplier = 3;
        [SerializeField, Tooltip("Коэффициент инерции при скролле")]
        private float afterDragInertia = 0.5f;
        /// <summary>
        /// Флаг активации процесса изменения позиции
        /// </summary>
        private bool _enableSlide;
        /// <summary>
        /// Флаг процесса считывания Drag мышью
        /// </summary>
        private bool _enableDrag;
        /// <summary>
        /// Изменение позиции при скролле
        /// </summary>
        private float _lastDragDelta;
        
        private void Awake()
        {
            var viewPortRect = ((RectTransform) transform.Find("Viewport")).rect;
            _viewPortSize = new Vector2(viewPortRect.width, viewPortRect.height);
    
            if (createOnAwake)
                InstantiateElements();
            if (subscribeOnAwake)
                AddClickCallback(delegate (int index) { Debug.Log(index); });
        }

        #region Instaintiate elements
        ///<summary>
        /// Создать элемент из перечисления
        /// </summary>
        public void InstantiateElements<T>(IEnumerable<T> values)
        {
            listElements.options.Clear();
            listElements.options.AddRange(values.Select(v=> new Dropdown.OptionData(v.ToString())));
            InstantiateElements();
        }
        
        ///<summary>
        /// Создать элементы из перечисления опций
        /// </summary>
        public void InstantiateElements(IEnumerable<dd.OptionData> elements)
        {
            listElements.options.Clear();
            listElements.options.AddRange(elements);
            InstantiateElements();
        }
        
        /// <summary>
        /// Создать элементы из DropDown-листа
        /// </summary>
        public void InstantiateElements(dd.OptionDataList elements)
        {
            listElements.options.Clear();
            listElements.options.AddRange(elements.options);
            InstantiateElements();
        }
        
        /// <summary>
        /// Создание элементов, позиционирование, расчет вспомогательных данных для опеределния сфокусированного элемента 
        /// </summary>
        private void InstantiateElements()
        {
            //Очищаем массив элементов
            if(_instantiatedElements != null && _instantiatedElements.Length > 0)
                foreach (var element in _instantiatedElements.ToArray())
                    Destroy(element);
            _instantiatedElements = new GameObject[ElementsCount];
            //Указываем границы долей позиции контенера от его ширины на каждый из элементов
            //Необходимо для определния индекса элемента после скролла
            _percentageBoardsOnElement = Extensions.PercentageBoardsOnElement(ElementsCount, elementSpaceWidthPercentage);
    
            //Генерируем элементы
            var startPosition = Vector2.zero;
            int maxOptionIndex;
            for (int i = 0; i <= (maxOptionIndex = ElementsCount - 1); i++)
            {
                FillElement(i, startPosition);
                if (i != maxOptionIndex)
                    startPosition += new Vector2(_viewPortSize.x * (1 + elementSpaceWidthPercentage), 0);
            }
    
            //Фиксируем размер (до начала последнего элемента) контейнера для отслеживания текущей позиции
            _containerWidth = startPosition.x;//берем текущую позицию, считаем долю от размера, получаем долю смещения, сверяемся с индексами элементов. Подробнее в работе
            //Ресайзим контейнер на ширину, покрывающей все элементы
            container.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _containerWidth + _viewPortSize.x);
            
            //Подписка на нажатие на элемент
            foreach (var btn in _instantiatedElements.Select(el => el.GetComponent<Button>()))
                btn.onClick.AddListener(delegate { onClickAction(ActiveIndex); });
        }
    
        /// <summary>
        /// Заполнение текстовых полей и картинок элемента исходя из значения опций
        /// </summary>
        private void FillElement(int index, Vector2 position)
        {
            var item = InstantiateElement(position, index);
            var data = listElements.options[index];
    
            if (!string.IsNullOrWhiteSpace(data.text))
                item.GetComponentInChildren<Text>().text = data.text;
            if (data.image != null)
                item.GetComponentInChildren<Image>().sprite = data.image;
        }
        
        /// <summary>
        /// Создание объекта-элемента и его позиционирование внутри контейнера
        /// </summary>
        private GameObject InstantiateElement(Vector2 position, int index)
        {
            var item = Instantiate(baseElement, container);
            var rect = item.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _viewPortSize.y);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _viewPortSize.x);
            rect.anchoredPosition = position;
    
            item.SetActive(true);
            _instantiatedElements[index] = item;
    
            return item;
        }
        #endregion
        
        #region Btns click move
        /// <summary>
        /// Нажатие на кнопку "Сместить влево"
        /// </summary>
        public void OnLeftClick()
        {
            if (ActiveIndex != 0)
                --ActiveIndex;
        
            StartCoroutine(SlideCor());
        }
        
        /// <summary>
        /// Нажатие на кнопку "Сместить вправо"
        /// </summary>
        public void OnRightClick()
        {
            if (ActiveIndex != ElementsCount - 1)
                ActiveIndex++;
                
            StartCoroutine(SlideCor());
        }
        
        /// <summary>
        /// Корутина слайда контейнера
        /// </summary>
        private IEnumerator SlideCor()
        {
            if(_enableSlide)
                yield break;
    
            _enableSlide = true;
            float t_start = Time.time, ct;
            Vector2 startPos = container.anchoredPosition,
                endPos = -ActiveObject.GetComponent<RectTransform>().anchoredPosition;
    
            while ((ct = (Time.time - t_start) / t_Slide) < 1f)
            {
                container.anchoredPosition = Vector2.Lerp(startPos, endPos, ct);
                yield return null;
            }
            container.anchoredPosition = endPos;
            _containerPositionPercentage = -endPos.x / _containerWidth;
            
            _enableSlide = false;
        }
        #endregion
        
        #region Scroll move
        /// <summary>
        /// При регистрации "Начала скролла" выдаем разрешения на скролл, устанавливаем дефолтные значений необходимых параметров
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(_enableSlide)
                return;
    
            _enableDrag = true;
            _lastDragDelta = 0;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if(!_enableDrag)
                return;
    
            var delta = eventData.delta.x * dragMultiplier;
            // Доля изменения позиции от всей ширины контейнера
            var deltaPercentage = delta / _containerWidth;
            // Изменение текущей доли, с перерасчетом на выходы за границы контейнера
            containerPositionPercentage -= deltaPercentage;
            // Изменение положения контейнера
            container.anchoredPosition = new Vector2(-_containerWidth * containerPositionPercentage, 0);
    
            _lastDragDelta += delta;    
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            StartCoroutine(AfterDragCor());
        }
    
        /// <summary>
        /// Коррутина инерциального движения после скролла
        /// </summary>
        private IEnumerator AfterDragCor()
        {
            float t_start = Time.time, ct;
            var startPos = container.anchoredPosition;
            var endPos = startPos + new Vector2(afterDragInertia*_lastDragDelta, 0);
    
            //Контроллируем, что инерциально движение не выдвигает элемент за границы
            var reduced = false;
            if (endPos.x > 0)
            {
                endPos = Vector2.zero;
                reduced = true;
            }
            else if (endPos.x < -_containerWidth)
            {
                endPos = new Vector2(-_containerWidth, 0);
                reduced = true;
            }

            while ((ct = (Time.time - t_start) / t_Inertia) < 1f)
            {
                container.anchoredPosition = Vector2.Lerp(startPos, endPos, ct);
                yield return null;
            }
    
            container.anchoredPosition = endPos;
            //Доля текущей позиции от всей ширины
            containerPositionPercentage = -endPos.x / _containerWidth;
            //ИНдекс активного элемента, исходя из доли текущей позиции
            ActiveIndex = _percentageBoardsOnElement.IndexAtBoards(_containerPositionPercentage);
            
            //Запуск корутины фокусирования на элементе
            if(!reduced)
                StartCoroutine(SlideCor());
            
            _lastDragDelta = 0;
            _enableDrag = false;
        }
        #endregion
        
        /// <summary>
        /// Подписаться на событие нажатия на элементы
        /// </summary>
        /// <param name="callbackAction">Действие при нажатии (на вход получает активный индекс элемента)</param>
        public void AddClickCallback(System.Action<int> callbackAction)
        {
            onClickAction += callbackAction;
        }

        /// <summary>
        /// Сброс выбранного значения
        /// </summary>
        public void ResetActive()
        {
            _containerPositionPercentage = 0;
            container.anchoredPosition = Vector2.zero;
            ActiveIndex = 0;
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Определение граничных долей каждого из элементов
        /// </summary>
        /// <param name="count">Количество элементов</param>
        /// <param name="spacePercentage">Доля промежуточного расстояния между элементами от ширины элементов</param>
        public static float[] PercentageBoardsOnElement(int count, float spacePercentage)
        {
            var result = new float[count];
            var sum = (1 + spacePercentage) * (count - 1);
            for (var i = 0; i < count; i++)
            {
                if (i == 0)
                    result[i] = (1 + spacePercentage / 2) / 2 / sum;
                else if(i == count - 1)
                    result[i] = 1f;
                else 
                    result[i] = (1 + (spacePercentage / 2)) / sum + result[i - 1];
            }
            return result;
        }
    
        /// <summary>
        /// Определение в какой "ячейке" находится текущая доля
        /// </summary>
        /// <param name="boards">Массив правых границ</param>
        /// <param name="percentage">Текущая доля</param>
        public static int IndexAtBoards(this float[] boards, float percentage)
        {
            //Начальная левая гранциа = 0. Конечная правая = 1. 
            //Происходит проврека слева направо, меньше ли текущее значение правой границы ячейки.
            //Если да, то текущая доля попадает в ячейку.
            for (var i = 0; i < boards.Length; i++)
                if (boards[i] > percentage)
                    return i;
    
            return boards.Length - 1;
        }
    }
}