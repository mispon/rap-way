using System;
using Game.UI.GameScreen;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Effects
{
    /// <summary>
    /// Эффект открытия нового тиммейта
    /// </summary>
    public class NewTeammateEffect : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SpriteRenderer teammateAvatar;
        [SerializeField] private Animation effect;

        private event Action onClose = () => {};
        
        /// <summary>
        /// Показывает эффект 
        /// </summary>
        public void Show(Sprite avatar, Action callback)
        {
            GameScreenController.Instance.SetVisibility(false);
            
            teammateAvatar.sprite = avatar;
            onClose = callback;
            
            gameObject.SetActive(true);
            effect.Play();
        }

        /// <summary>
        /// Обработчик клика 
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            GameScreenController.Instance.SetVisibility(true);
            onClose.Invoke();
            gameObject.SetActive(false);
        }
    }
}