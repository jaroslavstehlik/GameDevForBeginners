using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(StateBehaviour))]
    public class StateBehaviourEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        
        private void OnEnable()
        {
            ((StateBehaviour)target).OnValidate();
        }
    }
}