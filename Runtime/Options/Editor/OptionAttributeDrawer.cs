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
            int selectedIndex = Math.Max(options.GetOptionIndex(propertyObjectReferenceValue as Option), 0);
            string[] optionNames = options.optionNames;
            EditorGUI.showMixedValue = optionProperty.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(position, optionProperty.displayName, selectedIndex, optionNames);
            if (EditorGUI.EndChangeCheck())
            {
                optionProperty.serializedObject.Update();
                optionProperty.objectReferenceValue = options.options[selectedIndex];
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