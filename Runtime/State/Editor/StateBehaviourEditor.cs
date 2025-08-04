using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(StateBehaviour))]
    [CanEditMultipleObjects]
    public class StateBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (StateEditor.RenderStateField(serializedObject.FindProperty("_defaultOption"), target as IState))
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}