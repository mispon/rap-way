using Core;
using Core.Localization;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Base;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class SelectLanguageWindow : CanvasUIElement
    {
        [SerializeField] private Button ru;
        [SerializeField] private Button en;
        [SerializeField] private Button de;
        [SerializeField] private Button fr;
        [SerializeField] private Button it;
        [SerializeField] private Button es;
        [SerializeField] private Button pt;

        protected override void BeforeShow(object ctx = null)
        {
            ru.onClick.AddListener(() => SelectLang(GameLang.RU));
            en.onClick.AddListener(() => SelectLang(GameLang.EN));
            de.onClick.AddListener(() => SelectLang(GameLang.DE));
            fr.onClick.AddListener(() => SelectLang(GameLang.FR));
            it.onClick.AddListener(() => SelectLang(GameLang.IT));
            es.onClick.AddListener(() => SelectLang(GameLang.ES));
            pt.onClick.AddListener(() => SelectLang(GameLang.PT));

            base.BeforeShow(ctx);
        }

        private void SelectLang(GameLang lang) {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
            LocalizationManager.Instance.LoadLocalization(lang, true);
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.MainMenu));
        }
    }
}

