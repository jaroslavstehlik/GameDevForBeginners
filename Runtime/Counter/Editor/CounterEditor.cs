using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(Counter))]
    public class CounterEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}