using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI.ScrollViewController
{
    /// <summary>
    /// Класс управления отображения в ScrollView информации экзепляров Production
    /// Управляет компонентом RectTransform и отвечает за создание/активацию объектов информации
    /// </summary>
    public class ScrollViewController: MonoBehaviour
    {
        [Header("Контейнер элементов")] 
        [SerializeField] private int baseHeight;
        [SerializeField] private RectTransform container;

        [Header("Отступ между элементами")] 
        [SerializeField] private float spacing;

        /// <summary>
        /// Инициализация UI-элемента экземпляра Production
        /// </summary>
        public T InstantiatedElement<T>(GameObject template) where T: IScrollViewControllerItem
        {
            var newObject = Instantiate(template, container);
            newObject.SetActive(true);
            return newObject.GetComponent<T>();
        }

        /// <summary>
        /// Переопределение положения UI-элементов экземпляров Production
        /// </summary>
        public void RepositionElements<T>(List<T> itemControllers) where T: IScrollViewControllerItem
        {
            var itemsCount = itemControllers.Count;
            if (itemsCount == 0)
                return;
            
            foreach (var itemController in itemControllers)
                itemController.SetPosition(spacing);

            Resize(itemsCount, itemControllers.First().GetHeight());
        }

        /// <summary>
        /// Переопределение размера контейнера
        /// </summary>
        private void Resize(int itemsCount = 0, float height = 0)
        {
            container.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical, 
                Mathf.Max(baseHeight, (spacing + height) * itemsCount));
        }
    }
}