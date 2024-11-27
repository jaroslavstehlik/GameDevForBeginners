using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(StateConditionBehaviour))]
    public class StateConditionBehaviourEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        
        private void OnEnable()
        {
            ((StateConditionBehaviour)target).OnValidate();
        }
    }
}