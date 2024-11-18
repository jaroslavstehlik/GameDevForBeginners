using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(State))]
    public class StateEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}