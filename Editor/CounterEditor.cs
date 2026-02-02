using System.Reflection;
using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(Counter))]
    public class CounterEditor : Editor
    {
        public static void RenderCountField(ICountable icountable)
        {
            if(icountable == null || !EditorApplication.isPlaying)
                return;
            
            EditorGUI.BeginChangeCheck();
            float newValue = EditorGUILayout.FloatField(ObjectNames.NicifyVariableName(nameof(icountable.count)), icountable.count);
            if (!EditorGUI.EndChangeCheck())
                return;
            
            icountable.count = newValue;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            RenderCountField(target as ICountable);
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        
        private void OnEnable()
        {
            ((Counter)target).OnValidate();
        }
    }
}