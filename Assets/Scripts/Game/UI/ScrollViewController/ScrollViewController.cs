using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.ScrollViewController
{
    public class ScrollViewController: MonoBehaviour
    {
        [Header("Контейнер элементов")] 
        [SerializeField] private int baseHeight;
        [SerializeField] private int baseWidth;
        [SerializeField] private RectTransform container;

        [Header("Отступ между элементами")] 
        [SerializeField] private float spacing;

        [Header("Scroll direction")] 
        [SerializeField] private RectTransform.Axis direction;
        
        public T InstantiatedElement<T>(GameObject template) where T: IScrollViewControllerItem
        {
            var newObject = Instantiate(template, container);
            newObject.SetActive(true);
            return newObject.GetComponent<T>();
        }
        
        public void RepositionElements<T>(List<T> itemControllers) where T: IScrollViewControllerItem
        {
            var itemsCount = itemControllers.Count;
            if (itemsCount == 0)
                return;
            
            foreach (var itemController in itemControllers)
                itemController.SetPosition(spacing);

            if (direction == RectTransform.Axis.Vertical)
            {
                ResizeVertical(itemsCount, itemControllers.First().GetHeight());
            }
            else
            {
                ResizeHorizontal(itemsCount, itemControllers.First().GetWidth());
            }
        }

        private void ResizeVertical(int itemsCount = 0, float height = 0)
        {
            container.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical, 
                Mathf.Max(baseHeight, (spacing + height) * itemsCount));
        }
        
        private void ResizeHorizontal(int itemsCount = 0, float width = 0)
        {
            container.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, 
                Mathf.Max(baseWidth, (spacing + width) * itemsCount)
            );
        }
    }
}