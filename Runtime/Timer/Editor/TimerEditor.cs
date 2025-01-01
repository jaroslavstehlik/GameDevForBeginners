using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(Timer))]
    public class TimerEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}