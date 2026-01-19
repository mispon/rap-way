using System;
using System.Collections.Generic;
using RapWay.UI.Windows;
using UnityEngine;

namespace RapWay.Data
{
    [CreateAssetMenu(menuName = "RapWay/UI/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        [SerializeField] private List<BaseWindow> windowPrefabs;
        
        private Dictionary<Type, BaseWindow> _prefabsCache;

        public T GetPrefab<T>() where T : BaseWindow
        {
            return GetPrefab(typeof(T)) as T;
        }
        
        public BaseWindow GetPrefab(Type type)
        {
            if (_prefabsCache == null)
            {
                BuildCache();
            }

            if (_prefabsCache!.TryGetValue(type, out var window))
            {
                return window;
            }

            Debug.LogError($"[UIConfig] Префаб для окна типа {type} не найден в списке!");
            return null;
        }
        
        private void BuildCache()
        {
            _prefabsCache = new Dictionary<Type, BaseWindow>();

            foreach (var prefab in windowPrefabs)
            {
                if (prefab == null) continue;
                
                var type = prefab.GetType();
                
                if (!_prefabsCache.TryAdd(type, prefab))
                {
                    Debug.LogWarning($"[UIConfig] duplicated window: {type.Name}");
                }
            }
        }
    }
}