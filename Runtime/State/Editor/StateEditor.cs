using System;
using UnityEngine;
using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(State))]
    public class StateEditor : Editor
    {
        private SerializedProperty _defaultOption;
        private SerializedProperty _options;
        
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
            if (EditorGUI.EndChangeCheck())
            {
                serializedProperty.objectReferenceValue = state.activeOption = state.options.options[selectedIndex];
            }

            return true;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (RenderStateField(_defaultOption, target as IState))
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnEnable()
        {
            ((State)target).OnValidate();
            _defaultOption = serializedObject.FindProperty("_defaultOption");
            _options = serializedObject.FindProperty("_options");
            CheckDefaultOption(_defaultOption, _options);
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public static void CheckDefaultOption(SerializedProperty defaultOptionProperty, SerializedProperty optionsProperty)
        {
            if (!defaultOptionProperty.hasMultipleDifferentValues)
            {
                Options options = optionsProperty.objectReferenceValue as Options;
                if (options != null && options.Length() > 0)
                {
                    int index = options.GetOptionIndex(defaultOptionProperty.objectReferenceValue as Option);
                    if (index == -1)
                    {
                        defaultOptionProperty.objectReferenceValue = options[0];
                        defaultOptionProperty.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
}