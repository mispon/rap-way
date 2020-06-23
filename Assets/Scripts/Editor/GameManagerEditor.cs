using Core;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : UnityEditor.Editor
    {
        private GameManager _target;

        private void OnEnable()
        {
            _target = (GameManager) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);

            if (GUILayout.Button("Clear Prefs"))
            {
                _target.RemoveSaves();
            }
        }
    }
}