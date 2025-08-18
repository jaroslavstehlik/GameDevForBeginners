using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(StateBehaviour))]
    [CanEditMultipleObjects]
    public class StateBehaviourEditor : Editor
    {
        private SerializedProperty _defaultOption;
        private SerializedProperty _options;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (StateEditor.RenderStateField(serializedObject.FindProperty("_defaultOption"), target as IState))
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnEnable()
        {
            _defaultOption = serializedObject.FindProperty("_defaultOption");
            _options = serializedObject.FindProperty("_options");
            StateEditor.CheckDefaultOption(_defaultOption, _options);
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}