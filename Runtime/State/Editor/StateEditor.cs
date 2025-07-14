using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(State))]
    public class StateEditor : Editor
    {
        public static void RenderStateField(IState state)
        {
            if(state == null || !EditorApplication.isPlaying)
                return;
            
            int selectedIndex = Math.Max(state.options.GetOptionIndex(state.activeOption), 0);
            string[] optionNames = state.options.optionNames;
            if(optionNames.Length == 0)
                return;
            
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUILayout.Popup(ObjectNames.NicifyVariableName(nameof(state.activeOption)), selectedIndex, optionNames);
            if (!EditorGUI.EndChangeCheck())
                return;
            
            state.activeOption = state.options.options[selectedIndex];
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            RenderStateField(target as IState);
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