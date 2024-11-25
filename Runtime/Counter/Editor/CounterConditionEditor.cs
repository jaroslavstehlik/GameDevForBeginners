using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(CounterCondition))]
    public class CounterConditionEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        
        private void OnEnable()
        {
            ((CounterCondition)target).OnValidate();
        }
    }
}