using System;
using System.Collections;
using MessageBroker;
using MessageBroker.Messages.Game;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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
        private IDisposable _disposable;

        private void Awake()
        {
            value = GetComponent<Text>();

            _disposable = MsgBroker.Instance
                .Receive<LangChangedMessage>()
                .Subscribe(e => OnLangChanged());
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
        private void OnLangChanged()
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
            _disposable?.Dispose();
        }
    }
}