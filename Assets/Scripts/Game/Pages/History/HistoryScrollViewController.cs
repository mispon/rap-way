using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Pages.History
{
    /// <summary>
    /// Класс управления отображения в ScrollView информации экзепляров Production
    /// Управляет компонентом RectTransform и отвечает за создание/активацию объектов информации
    /// </summary>
    public class HistoryScrollViewController: MonoBehaviour
    {
        [Header("Контейнер элементов")] 
        [SerializeField] private RectTransform container;

        [Header("Отступ между элементами")] 
        [SerializeField] private float spacing;
        
        /// <summary>
        /// Высота контейнера по умолчанию
        /// </summary>
        private float _baseHeight;

        private void Start()
        {
            _baseHeight = container.parent.GetComponent<RectTransform>().rect.height;
            Resize();
        }

        /// <summary>
        /// Инициализация UI-элемента экземпляра Production
        /// </summary>
        public HistoryInfoItemController InstantiatedElement(GameObject template)
        {
            var newObject = Instantiate(template, container);
            newObject.SetActive(true);
            return newObject.GetComponent<HistoryInfoItemController>();
        }

        /// <summary>
        /// Переопределение положения UI-элементов экземпляров Production
        /// </summary>
        public void RepositionElements(List<HistoryInfoItemController> itemControllers)
        {
            var itemsCount = itemControllers.Count;
            if (itemsCount == 0)
                return;
            
            foreach (var itemController in itemControllers)
                itemController.SetPosition(spacing);

            Resize(itemsCount, itemControllers.First().Height);
        }

        /// <summary>
        /// Переопределение размера контейнера
        /// </summary>
        private void Resize(int itemsCount = 0, float height = 0)
        {
            container.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical, 
                Mathf.Max(_baseHeight, (spacing + height) * itemsCount));
        }
    }
}