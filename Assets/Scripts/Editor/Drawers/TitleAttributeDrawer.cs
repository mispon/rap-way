using Core.PropertyAttributes;
using UnityEditor;
using UnityEngine;

namespace Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ElementTitleAttribute))]
    public class ElementTitleAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property,
            GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        protected virtual ElementTitleAttribute Atribute
        {
            get { return (ElementTitleAttribute)attribute; }
        }

        SerializedProperty TitleNameProp;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, new GUIContent(Atribute.draw_name, label.tooltip), false);
        }
    }
}
