using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(StateBehaviour))]
    [CanEditMultipleObjects]
    public class StateBehaviourEditor : Editor
    {
        private SerializedProperty onStateCreatedProperty;
        private SerializedProperty onStateChangedProperty;
        private SerializedProperty stateDescriptorProperty;

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        
        private void OnEnable()
        {
            onStateCreatedProperty = serializedObject.FindProperty("_onStateCreated");
            onStateChangedProperty = serializedObject.FindProperty("_onStateChanged");
            stateDescriptorProperty = serializedObject.FindProperty("_stateDescriptor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(stateDescriptorProperty.FindPropertyRelative("name"));
            if (targets.Length == 1)
            {
                StateBehaviour stateBehaviour = target as StateBehaviour;
                if (stateBehaviour.state != null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.LabelField(nameof(stateBehaviour.state.activeState), stateBehaviour.state.activeState);
                    EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUILayout.PropertyField(stateDescriptorProperty.FindPropertyRelative("defaultState"));
            // TODO: add state selection dropdown
            EditorGUILayout.PropertyField(stateDescriptorProperty.FindPropertyRelative("states"));
            EditorGUILayout.PropertyField(stateDescriptorProperty.FindPropertyRelative("saveKey"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(onStateCreatedProperty);
            EditorGUILayout.PropertyField(onStateChangedProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}