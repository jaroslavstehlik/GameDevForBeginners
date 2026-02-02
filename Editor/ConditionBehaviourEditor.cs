using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(ConditionBehaviour))]
    public class ConditionBehaviourEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        
        private void OnEnable()
        {
            ((ConditionBehaviour)target).OnValidate();
        }
    }
}