using UnityEngine;
using UnityEditor;

namespace Editor
{
    /// <summary>
    /// CheckCondition out http://diegogiacomelli.com.br/unitytips-hierarchy-window-group-header/
    /// </summary>
    [InitializeOnLoad]
    public class HierarchyLabel : MonoBehaviour
    {
        static HierarchyLabel()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            
            if (obj == null)
                return;

            if (obj.name.StartsWith("---", System.StringComparison.Ordinal))
            {
                EditorGUI.DrawRect(selectionRect, Color.grey);
                EditorGUI.DropShadowLabel(selectionRect, obj.name.Replace("-", ""));
            }
        }
    }
}