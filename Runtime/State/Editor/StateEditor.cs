using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(State))]
    public class StateEditor : Editor
    {
        public static bool RenderStateField(SerializedProperty serializedProperty, IState state)
        {
            if(state == null || !EditorApplication.isPlaying)
                return false;
            
            int selectedIndex = Math.Max(state.options.GetOptionIndex(state.activeOption), 0);
            string[] optionNames = state.options.optionNames;
            if(optionNames.Length == 0)
                return false;
            
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUILayout.Popup(ObjectNames.NicifyVariableName(nameof(state.activeOption)), selectedIndex, optionNames);
            if (!EditorGUI.EndChangeCheck())
                return false;
            
            serializedProperty.objectReferenceValue = state.activeOption = state.options.options[selectedIndex];
            return true;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (RenderStateField(serializedObject.FindProperty("_defaultOption"), target as IState))
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnEnable()
        {
            ((State)target).OnValidate();
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}