namespace UnityEditor.Localization.Bridge
{
    static class InspectorWindowBridge
    {
        public static void Repaint()
        {
            // This is used to repaint the inspector window when the selection changes.
            // It is used by the LocalizedReferencePicker to ensure the inspector updates
            // when a new reference is selected.
            InspectorWindow.RepaintAllInspectors();
        }
    }
}
