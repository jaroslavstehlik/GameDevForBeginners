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

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}