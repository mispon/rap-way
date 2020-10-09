using System;
using Core;
using Game.UI.GameScreen;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Effects
{
    /// <summary>
    /// Эффект открытия нового тиммейта
    /// </summary>
    public class NewItemEffect : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SpriteRenderer itemAvatar;
        [SerializeField] private Animation effect;
        [SerializeField] private GameObject fireWorkObject;
        
        private event Action onClose = () => {};
        
        /// <summary>
        /// Показывает эффект 
        /// </summary>
        public void Show(Sprite avatar, [CanBeNull] Action callback, bool fireWorks = false)
        {
            CanvasController.SetActive(false);
            
            itemAvatar.sprite = avatar;
            onClose = callback;
            
            gameObject.SetActive(true);
            fireWorkObject.SetActive(fireWorks);
            
            effect.Play();
        }

        /// <summary>
        /// Обработчик клика 
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            CanvasController.SetActive(true);
            SoundManager.Instance.PlayClick();
            GameScreenController.Instance.SetVisibility(true);
            onClose.Invoke();
            fireWorkObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}