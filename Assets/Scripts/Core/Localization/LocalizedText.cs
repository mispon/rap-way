using System.Collections;
using Core.Events;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace Core.Localization
{
    public enum TextCase
    {
        Normal,
        Lower,
        Upper
    }
    
    /// <summary>
    /// Локализируемый текстовый элемент сцены
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private TextCase textCase = TextCase.Normal;
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
            
            ApplyText();
        }

        /// <summary>
        /// Обработчик изменения языка
        /// </summary>
        private void OnLangChanged(object[] args)
        {
            ApplyText();
        }

        /// <summary>
        /// Устанавливает текст 
        /// </summary>
        private void ApplyText()
        {
            string text = LocalizationManager.Instance.Get(key);
            
            if (textCase == TextCase.Lower)
                text = text.ToLowerInvariant();
            else if (textCase == TextCase.Upper)
                text = text.ToUpperInvariant();

            value.text = text;
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.LangChanged, OnLangChanged);
        }
    }
}