using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(CounterBehaviour))]
    [CanEditMultipleObjects]
    public class CounterBehaviourEditor : Editor
    {
        private SerializedProperty onCounterCreatedProperty;
        private SerializedProperty onCountChangedProperty;
        private SerializedProperty counterDescriptorProperty;

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private void OnEnable()
        {
            onCounterCreatedProperty = serializedObject.FindProperty("_onCounterCreated");
            onCountChangedProperty = serializedObject.FindProperty("_onCountChanged");
            counterDescriptorProperty = serializedObject.FindProperty("_counterDescriptor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(counterDescriptorProperty.FindPropertyRelative("name"));
            if (targets.Length == 1)
            {
                CounterBehaviour counterBehaviour = target as CounterBehaviour;
                if (counterBehaviour.counter != null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.FloatField(nameof(counterBehaviour.counter.count), counterBehaviour.counter.count);
                    EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUILayout.PropertyField(counterDescriptorProperty.FindPropertyRelative("defaultCount"));
            EditorGUILayout.PropertyField(counterDescriptorProperty.FindPropertyRelative("wholeNumber"));
            EditorGUILayout.PropertyField(counterDescriptorProperty.FindPropertyRelative("saveKey"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(onCounterCreatedProperty);
            EditorGUILayout.PropertyField(onCountChangedProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}