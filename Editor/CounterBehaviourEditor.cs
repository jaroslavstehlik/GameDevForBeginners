using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(CounterBehaviour))]
    public class CounterBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CounterEditor.RenderCountField(target as ICountable);
        }

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