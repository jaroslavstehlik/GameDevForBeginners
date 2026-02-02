using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(Condition))]
    public class ConditionEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        
        private void OnEnable()
        {
            ((Condition)target).OnValidate();
        }
    }
}