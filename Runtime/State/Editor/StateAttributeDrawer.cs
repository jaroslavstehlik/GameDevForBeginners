using UnityEditor;
using UnityEngine;

namespace GameDevForBeginners
{
    [CustomPropertyDrawer(typeof(StateAttribute))]
    public class StateAttributeDrawer : PropertyDrawer
    {
        static void RenderProperty(Rect position, SerializedProperty property)
        {
            EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
        }

        string[] FindOptions(SerializedProperty property)
        {
            SerializedProperty stateProperty = property.serializedObject.FindProperty("state");
            if (stateProperty != null)
            {
                State state = stateProperty.objectReferenceValue as State;
                if (state != null)
                {
                    return (string[])state.states.Clone();
                }
            }

            SerializedProperty statesProperty = property.serializedObject.FindProperty("states");
            if (statesProperty != null)
            {
                string[] options = new string[statesProperty.arraySize];
                for (int i = 0; i < options.Length; i++)
                {
                    options[i] = statesProperty.GetArrayElementAtIndex(i).stringValue;
                }

                return options;
            }

            return null;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string[] options = FindOptions(property);
            if (options == null || options.Length == 0)
            {
                RenderProperty(position, property);
                return;
            }

            string selectedString = property.stringValue;
            int selectedIndex = 0;
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] != selectedString)
                    continue;

                selectedIndex = i;
                break;
            }

            selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, options);
            property.stringValue = options[selectedIndex];
        }
    }
}