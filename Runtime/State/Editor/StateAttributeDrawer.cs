using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
            int selectedIndex = Math.Max(options.GetOptionIndex(propertyObjectReferenceValue as Option), 0);
            string[] optionNames = options.optionNames;
            
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, optionNames);
            if(EditorGUI.EndChangeCheck())
                property.objectReferenceValue = options.options[selectedIndex];
            EditorGUI.showMixedValue = false;
        }
    }
}