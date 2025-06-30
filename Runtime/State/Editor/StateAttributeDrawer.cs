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

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            StateAttribute stateAttribute = attribute as StateAttribute;
            SerializedProperty stateProperty = property.serializedObject.FindProperty(stateAttribute.fieldName);
            if (stateProperty == null)
            {
                RenderProperty(position, property);
                return;
            }
            
            SerializedProperty stateValueProperty = stateProperty.FindPropertyRelative("_value");
            if (stateValueProperty == null || stateValueProperty.objectReferenceValue == null)
            {
                RenderProperty(position, property);
                return;
            }
            
            IState state = stateValueProperty.objectReferenceValue as IState;
            Options options = state.options;
            
            if (options == null || options.options.Length == 0)
            {
                RenderProperty(position, property);
                return;
            }

            Object propertyObjectReferenceValue = property.objectReferenceValue;
            int selectedIndex = 0;
            for (int i = 0; i < options.options.Length; i++)
            {
                if (options.options[i] != propertyObjectReferenceValue)
                    continue;

                selectedIndex = i;
                break;
            }

            string[] optionNames = new string[options.options.Length];
            for (int i = 0; i < optionNames.Length; i++)
            {
                optionNames[i] = options.options[i].name;
            }
            selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, optionNames);
            property.objectReferenceValue = options.options[selectedIndex];
        }
    }
}