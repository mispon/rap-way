using System;
using UnityEngine;
using Utils.Extensions;

namespace Models.UI
{
    /// <summary>
    /// Фиксированные настройки Канвас-группы
    /// </summary>
    [Serializable]
    public struct CanvasGroupSettings
    {
        public float alpha;
        public bool interactable;
        public bool blocksRaycasts;
        public bool ignoreParentGroups;
    }

    /// <summary>
    /// Класс управления параметрами канвас-группы
    /// </summary>
    [Serializable]
    public class CanvasGroupController
    {
        public CanvasGroup canvasGroup;
        
        [Space, SerializeField]
        private CanvasGroupSettings showSettings;
        [SerializeField]
        private CanvasGroupSettings hideSettings;

        public void SetActive(bool value)
        {
            canvasGroup.Set(value ? showSettings : hideSettings);
        }
    }
}