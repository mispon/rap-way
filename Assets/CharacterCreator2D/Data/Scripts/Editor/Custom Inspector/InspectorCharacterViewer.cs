using CharacterCreator2D;
using UnityEditor;
using UnityEngine;

namespace CharacterEditor2D
{
    [CustomEditor(typeof(CharacterViewer))]
    public class CharacterViewerEditor : UnityEditor.Editor
    {
        private CharacterViewer    _character;
        private SerializedProperty _initialized;
        private SerializedProperty _instancemat;
        private SerializedProperty _tintcolor;
        private bool               _hidechild;

        private readonly GUIContent _initializedgui = new()
        {
            text = "Initialize On Awake",
            tooltip =
                "Initialized the CharacterViewer at awake. You can turn this off to save performance if you don't have any plan to customize this character at runtime."
        };
        private readonly GUIContent _instancematgui = new()
        {
            text = "Instantiate Material",
            tooltip =
                "Instantiate materials for each character when the application is playing. You can turn this off to save performance. Turning this off will make every instance of the prefab uses shared materials"
        };
        private readonly GUIContent _tintgui = new()
        {
            text    = "Tint Color",
            tooltip = "Character's tint color"
        };
        private readonly GUIContent _hidechildui = new()
        {
            text    = "Auto Hide Children",
            tooltip = "Automatically hide children in hierarchy (Shared settings)"
        };

        private void OnEnable()
        {
            _character   = (CharacterViewer) target;
            _initialized = serializedObject.FindProperty("initializeAtAwake");
            _instancemat = serializedObject.FindProperty("instanceMaterials");
            _tintcolor   = serializedObject.FindProperty("_tintcolor");
            _hidechild   = EditorPrefs.GetBool("Simpleton/CC2D/autohidechild", true);
            if (_hidechild)
            {
                HideInHierarchy(_character);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var tcolor = _character.TintColor;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings", WizardUtils.BoldTextStyle);
            EditorGUILayout.PropertyField(_initialized, _initializedgui);
            EditorGUILayout.PropertyField(_instancemat, _instancematgui);
            EditorGUILayout.PropertyField(_tintcolor, _tintgui);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Hierarchy", WizardUtils.BoldTextStyle);
            _hidechild = EditorGUILayout.Toggle(_hidechildui, _hidechild);
            EditorPrefs.SetBool("Simpleton/CC2D/autohidechild", _hidechild);

            if (GUILayout.Button("Show Child"))
            {
                ShowInHierarchy(_character);
            }

            if (GUILayout.Button("Hide Child"))
            {
                HideInHierarchy(_character);
            }

            BakeButtons();
            serializedObject.ApplyModifiedProperties();

            if (tcolor != _tintcolor.colorValue)
            {
                _character.RepaintTintColor();
            }
        }

        private void BakeButtons()
        {
            if (Application.isPlaying)
            {
                if (_character.IsBaked)
                {
                    if (GUILayout.Button("Rebake"))
                    {
                        _character.Bake();
                    }

                    if (GUILayout.Button("Unbake"))
                    {
                        _character.Unbake();
                    }
                } else
                {
                    if (GUILayout.Button("Bake"))
                    {
                        _character.Bake();
                    }
                }
            }
        }

        private void ShowInHierarchy(CharacterViewer character)
        {
            var child = character.GetComponentsInChildren<Transform>(true);
            foreach (var c in child)
            {
                c.gameObject.hideFlags = HideFlags.None;
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        private void HideInHierarchy(CharacterViewer character)
        {
            var child = character.GetComponentsInChildren<Transform>(true);
            foreach (var c in child)
            {
                if (c != child[0])
                {
                    c.gameObject.hideFlags =  HideFlags.HideInInspector;
                    c.gameObject.hideFlags ^= HideFlags.HideInHierarchy;
                }
            }

            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.DirtyHierarchyWindowSorting();
        }
    }
}