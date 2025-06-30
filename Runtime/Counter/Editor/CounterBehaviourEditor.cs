using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(CounterBehaviour))]
    public class CounterBehaviourEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        
        private void OnEnable()
        {
            ((CounterBehaviour)target).OnValidate();
        }
    }
}