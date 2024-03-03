using System;
using Core;
using MessageBroker;
using MessageBroker.Messages.Game;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controls.Carousel
{
    public class Carousel : MonoBehaviour
    {
        [Header("Arrows")]
        [SerializeField] private Button leftArrow;
        [SerializeField] private Button rightArrow;

        [Header("Settings")]
        [SerializeField] private Transform itemsContainer;
        [SerializeField] private CarouselItem itemPrefab;
        [SerializeField] private CarouselProps[] props;
        [SerializeField] private bool onAwake;
        
        public event Action<int> onChange = index => {}; 
        public event Action<int> onClick = index => {};

        private int _index;
        private CarouselItem[] _items;
        private IDisposable _disposable;
        
        private void Start()
        {
            leftArrow.onClick.AddListener(() => OnArrowClicked(-1));
            rightArrow.onClick.AddListener(() => OnArrowClicked(1));

            if (onAwake) 
                _disposable = MsgBroker.Instance
                    .Receive<GameReadyMessage>()
                    .Subscribe(_ => Init());
        }
        
        private void Init() => Init(props);

        public void Init(CarouselProps[] itemProps)
        {
            Clear();

            _index = 0;
            _items = new CarouselItem[itemProps.Length];
            
            for (var i = 0; i < itemProps.Length; i++)
            {
                var item = Instantiate(itemPrefab, itemsContainer);
                item.Setup(itemProps[i], onElementClicked);
                item.name = $"Item-{i}";
                
                _items[i] = item;
            }
            
            UpdateItems();
        }

        public T GetValue<T>() => _items[_index].GetValue<T>();

        public string GetLabel() => _items[_index].GetLabel();

        public void SetIndex(int index)
        {
            _index = index;
            OnArrowClicked(0, silent: true);
        }

        public void SetIndex(string label)
        {
            int index = 0;
            foreach (var item in _items)
            {
                if (string.Equals(label, item.GetLabel(), StringComparison.InvariantCultureIgnoreCase))
                {
                    SetIndex(index);
                    break;
                }

                index++;
            }
        }

        private void Clear()
        {
            if (_items == null)
                return;
            
            foreach (var item in _items)
                Destroy(item.gameObject);
        }

        private void onElementClicked()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            onClick.Invoke(_index);
        }

        private void OnArrowClicked(int direction, bool silent = false)
        {
            if (!silent)
                SoundManager.Instance.PlaySound(UIActionType.Switcher);
            
            _index += direction;
            
            ClampIndex();
            UpdateItems();
            
            onChange.Invoke(_index);
        }

        private void ClampIndex()
        {
            if (_index < 0)
                _index = _items.Length - 1;
            
            if (_index == _items.Length)
                _index = 0;
        }

        private void UpdateItems()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                _items[i].gameObject.SetActive(i == _index);
            }
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
} 