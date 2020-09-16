using Models.UI;
using UnityEngine;

namespace Utils.Extensions
{
    public static class CanvasExtension
    {
        /// <summary>
        /// Устанавливает параметры для CanvasGroup 
        /// </summary>
        public static void Set(this CanvasGroup canvasGroup, CanvasGroupSettings settings)
        {
            canvasGroup.alpha = settings.alpha;
            canvasGroup.interactable = settings.interactable;
            canvasGroup.blocksRaycasts = settings.blocksRaycasts;
            canvasGroup.ignoreParentGroups = settings.ignoreParentGroups;
        }
    }
}