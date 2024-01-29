using UI.Models;
using UnityEngine;

namespace Extensions
{
    public static class CanvasExtension
    {
        /// <summary>
        /// Устанавливает параметры для CanvasGroup 
        /// </summary>
        public static void Set(this CanvasGroup canvasGroup, in CanvasGroupSettings settings)
        {
            canvasGroup.alpha = settings.alpha;
            canvasGroup.interactable = settings.interactable;
            canvasGroup.blocksRaycasts = settings.blocksRaycasts;
            canvasGroup.ignoreParentGroups = settings.ignoreParentGroups;
        }
    }
}