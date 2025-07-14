using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(StateBehaviour))]
    [CanEditMultipleObjects]
    public class StateBehaviourEditor : Editor
    {
        private SerializedProperty nameProperty;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            StateEditor.RenderStateField(target as IState);
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}