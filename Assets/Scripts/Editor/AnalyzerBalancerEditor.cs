using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor
{
    [CustomEditor(typeof(AnalyzersBalancer))]
    public class AnalyzerBalancerEditor : UnityEditor.Editor
    {
        private AnalyzersBalancer _target;

        private void OnEnable()
        {
            _target = (AnalyzersBalancer) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);

            if (GUILayout.Button("Analyze track"))
            {
                _target.AnalyzeTrack();
            }

            if (GUILayout.Button("Analyze album"))
            {
                _target.AnalyzeAlbum();
            }

            if (GUILayout.Button("Analyze clip"))
            {
                _target.AnalyzeClip();
            }

            if (GUILayout.Button("Analyze concert"))
            {
                _target.AnalyzeConcert();
            }

            if (GUILayout.Button("Analyze socials"))
            {
                _target.AnalyzeSocial();
            }
        }
    }
}