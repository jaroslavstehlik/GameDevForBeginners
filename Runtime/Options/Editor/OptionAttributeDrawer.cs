using UnityEditor;
using UnityEngine;

namespace GameDevForBeginners
{
    [CustomPropertyDrawer(typeof(OptionAttribute))]
    public class OptionAttributeDrawer : PropertyDrawer
    {
        static void RenderProperty(Rect position, SerializedProperty property)
        {
            EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OptionAttribute optionAttribute = attribute as OptionAttribute;
            SerializedProperty optionsProperty = property.serializedObject.FindProperty(optionAttribute.fieldName);
            Options options = optionsProperty.objectReferenceValue as Options;
            
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
            
            property.serializedObject.Update();
            property.objectReferenceValue = options.options[selectedIndex];
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}