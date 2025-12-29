using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameDevForBeginners
{
    [CustomPropertyDrawer(typeof(OptionAttribute))]
    public class OptionAttributeDrawer : PropertyDrawer
    {
        public static void RenderOptionPropertyGUI(Rect position, SerializedProperty optionProperty, SerializedProperty optionsProperty)
        { 
            Options options = optionsProperty.objectReferenceValue as Options;
            if (options == null || options.options.Length == 0)
            {
                RenderPropertyGUI(position, optionProperty);
                return;
            }
            
            Object propertyObjectReferenceValue = optionProperty.objectReferenceValue;
            int selectedIndex = options.GetOptionIndex(propertyObjectReferenceValue as Option);

            // handle default none selection.
            string[] optionNames = new string[options.optionNames.Length + 1];
            optionNames[0] = "none";
            for (int i = 0; i < options.optionNames.Length; i++)
            {
                optionNames[i + 1] = options.optionNames[i];
            }
            
            EditorGUI.showMixedValue = optionProperty.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(position, optionProperty.displayName, selectedIndex + 1, optionNames) - 1;
            if (EditorGUI.EndChangeCheck())
            {
                optionProperty.serializedObject.Update();
                if(selectedIndex < 0)
                {
                    optionProperty.objectReferenceValue = null;    
                } else {
                    optionProperty.objectReferenceValue = options.options[selectedIndex];
                }
                optionProperty.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.showMixedValue = false;
        }
        
        static void RenderPropertyGUI(Rect position, SerializedProperty property)
        {
            EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
        }
        
        static void RenderPropertyGUILayout(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, new GUIContent(property.displayName));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OptionAttribute optionAttribute = attribute as OptionAttribute;
            SerializedProperty optionsProperty = property.serializedObject.FindProperty(optionAttribute.fieldName);
            RenderOptionPropertyGUI(position, property, optionsProperty);
        }
    }
}