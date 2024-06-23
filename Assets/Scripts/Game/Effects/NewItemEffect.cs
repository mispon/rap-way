using System;
using Core;
using JetBrains.Annotations;
using ScriptableObjects;
using UI.GameScreen;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Effects
{
    public class NewItemEffect : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SpriteRenderer itemAvatar;
        [SerializeField] private Animation effect;

        [Header("Дополнительные эффекты")]
        [SerializeField] private GameObject haloLight;
        
        private event Action onClose = () => {};

        public void Show(Sprite avatar, [CanBeNull] Action callback)
        {
            CanvasController.SetActive(false);
            
            itemAvatar.sprite = avatar;
            onClose = callback;
            
            gameObject.SetActive(true);
            haloLight.SetActive(true);
            
            effect.Play();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            CanvasController.SetActive(true);
            GameScreenController.Instance.SetVisibility(true);
            
            haloLight.SetActive(false);
            gameObject.SetActive(false);
            
            onClose.Invoke();
        }
    }
}