using DG.Tweening;
using RapWay.Core.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace RapWay.UI.Widgets
{
    [RequireComponent(typeof(Button))]
    public class BaseButton : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string clickSoundId = "btn_click";
        
        [Header("Animation")]
        [SerializeField] private float punchDuration = 0.2f; // Длительность блокировки и анимации
        [SerializeField] private float punchStrength = 0.1f; // Сила "сжатия" (0.1 = 10%)
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float elasticity = 1f;
        
        private Button _button;
        private AudioService _audioService;
        private Tween _clickTween;

        private readonly UnityEvent _onSafeClick = new();
        
        [Inject]
        public void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            
            // disable standard arrows navigation
            var nav = _button.navigation;
            nav.mode = Navigation.Mode.None;
            _button.navigation = nav;

            // subs to native click
            _button.onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            if (_clickTween != null && _clickTween.IsActive() && _clickTween.IsPlaying())
                return;
            
            _audioService?.PlaySfx(clickSoundId);
            
            transform.localScale = Vector3.one;
            _clickTween = transform.DOPunchScale(Vector3.one * punchStrength, punchDuration, vibrato, elasticity)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
            
            _onSafeClick.Invoke();
            
        }
        
        public void AddListener(UnityAction action)
        {
            _onSafeClick.AddListener(action);
        }

        public void RemoveAllListeners()
        {
            _onSafeClick.RemoveAllListeners();
        }

        public void SetInteractable(bool state)
        {
            _button.interactable = state;
            
            if (!state)
            {
                transform.DOKill();
                transform.localScale = Vector3.one;
            }
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}