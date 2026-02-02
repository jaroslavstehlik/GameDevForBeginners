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
            int selectedIndex = options.GetOptionIndex(propertyObjectReferenceValue as Option);

            // handle default none selection.
            string[] optionNames = new string[options.optionNames.Length + 1];
            optionNames[0] = "none";
            for (int i = 0; i < options.optionNames.Length; i++)
            {
                optionNames[i + 1] = options.optionNames[i];
            }
            
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex + 1, optionNames) - 1;
            if(EditorGUI.EndChangeCheck()) {
                property.serializedObject.Update();
                if(selectedIndex < 0)
                {
                    property.objectReferenceValue = null;    
                } else {
                    property.objectReferenceValue = options.options[selectedIndex];
                }
                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.showMixedValue = false;
        }
    }
}