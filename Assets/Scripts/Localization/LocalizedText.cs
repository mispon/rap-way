using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.EventType;

namespace Localization
{
    /// <summary>
    /// Локализируемый текстовый элемент сцены
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string key;
        
        private Text value;
        
        private void Awake()
        {
            value = GetComponent<Text>();
            EventManager.AddHandler(EventType.LangChanged, OnLangChanged);
        }

        private IEnumerator Start()
        {
            while (!LocalizationManager.Instance.IsReady)
                yield return null;
            
            value.text = LocalizationManager.Instance.Get(key);
        }

        /// <summary>
        /// Обработчик изменения языка
        /// </summary>
        private void OnLangChanged(object[] args)
        {
            value.text = LocalizationManager.Instance.Get(key);
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.LangChanged, OnLangChanged);
        }
    }
}