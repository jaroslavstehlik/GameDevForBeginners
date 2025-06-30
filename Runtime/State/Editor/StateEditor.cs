using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(State))]
    public class StateEditor : Editor
    {
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