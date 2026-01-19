using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RapWay.Data;
using RapWay.UI.Windows;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RapWay.Core.UI
{
    public class UIService
    {
        private readonly UIConfig _config;
        private readonly IObjectResolver _container; // VContainer for windows creation
        private readonly Transform _uiRoot;

        private readonly Dictionary<Type, BaseWindow> _windows = new();
        private readonly Stack<BaseWindow> _history = new();
        
        // Layers (Z-order)
        private const int ORDER_HUD = 0;
        private const int ORDER_SCREEN = 100;
        private const int ORDER_POPUP = 200;
        private const int ORDER_NOTIFICATION = 500;
        
        public UIService(UIConfig config, IObjectResolver container)
        {
            _config = config;
            _container = container;
            
            var existingRoot = GameObject.Find("UI Root");
            _uiRoot = existingRoot ? existingRoot.transform : new GameObject("UI Root").transform;
        }

        public async UniTask Open<T>(object payload = null) where T : BaseWindow
        {
            await Open(typeof(T), payload);
        }
        
        public async UniTask Open(Type windowType, object payload = null)
        {
            if (!typeof(BaseWindow).IsAssignableFrom(windowType))
            {
                Debug.LogError($"[UIService] {windowType.Name} does not implement BaseWindow");
                return;
            }
            
            var window = GetOrCreateWindow(windowType);
            
            switch (window.Type)
            {
                case WindowType.HUD:
                    window.SetSortingOrder(ORDER_HUD); 
                    break;
                
                case WindowType.Screen:
                    {
                        if (_history.Count > 0)
                        {
                            var current = _history.Peek();
                            await current.Hide(); 
                        }
                
                        window.SetSortingOrder(ORDER_SCREEN);
                        _history.Push(window);
                        break;
                    }
                
                case WindowType.Popup:
                    window.SetSortingOrder(ORDER_POPUP + _history.Count);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException($"[UIService] Unexpexted window type: {window.Type}");
            }

            await window.Show(payload);
        }
        
        public async UniTask Back()
        {
            if (_history.Count < 1) return;

            var current = _history.Pop();
            await current.Hide();

            if (_history.Count > 0)
            {
                var previous = _history.Peek();
                await previous.Show();
            }
        }
        
        private BaseWindow GetOrCreateWindow(Type type)
        {
            if (_windows.TryGetValue(type, out var window))
            {
                return window;
            }

            var prefab = _config.GetPrefab(type);
            if (prefab == null)
            {
                throw new Exception($"[UIService] Prefab for UI {type.Name} not found in UIConfig!");
            }

            var instance = _container.Instantiate(prefab, _uiRoot);
            _windows.Add(type, instance);
            
            return instance;
        }
        
        public void ShowNotification(string message, bool isError = false)
        {
            // TODO: implement notifications
            Debug.Log($"[UIService] Notification: {message}");
        }
    }
}