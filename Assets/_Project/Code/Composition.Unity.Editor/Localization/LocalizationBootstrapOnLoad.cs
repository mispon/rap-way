using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization.Settings;

namespace RapWay.Composition.Unity.Editor.Localization
{
    [InitializeOnLoad]
    public static class LocalizationBootstrapOnLoad
    {
        private const string SessionKey = "RapWay.LocalizationBootstrapOnLoad.Completed";

        static LocalizationBootstrapOnLoad()
        {
            if (SessionState.GetBool(SessionKey, false))
            {
                return;
            }

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += TryBootstrap;
        }

        private static void TryBootstrap()
        {
            if (LocalizationEditorSettings.ActiveLocalizationSettings != null &&
                LocalizationEditorSettings.GetStringTableCollection(Presentation.Unity.Localization.GameLocalizationTables.UiShell) != null)
            {
                return;
            }

            LocalizationCatalogBootstrapper.BootstrapCatalog();
        }
    }
}
