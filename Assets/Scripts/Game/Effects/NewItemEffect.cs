using System;
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

        private event Action onClose = () => {};
        
        /// <summary>
        /// Показывает эффект 
        /// </summary>
        public void Show(Sprite avatar, [CanBeNull] Action callback)
        {
            CanvasController.SetActive(false);
            
            itemAvatar.sprite = avatar;
            onClose = callback;
            
            gameObject.SetActive(true);
            effect.Play();
        }

        /// <summary>
        /// Обработчик клика 
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            CanvasController.SetActive(true);
            onClose?.Invoke();
            gameObject.SetActive(false);
        }
    }
}