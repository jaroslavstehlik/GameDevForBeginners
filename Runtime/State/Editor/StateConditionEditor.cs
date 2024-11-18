using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(StateCondition))]
    public class StateConditionEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}