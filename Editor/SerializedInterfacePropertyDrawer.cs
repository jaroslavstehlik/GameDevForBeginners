using System;
using UnityEditor;
using UnityEngine;

namespace GameDevForBeginners
{
    [CustomPropertyDrawer(typeof(SerializedInterfaceAttribute))]
    public class SerializedInterfacePropertyDrawer : PropertyDrawer
    {
        static string GetErrorMessage(Type referenceType, Type interfaceType)
        {
            return $"Wrong type: {referenceType}, expected: {interfaceType}";
        }

        static GUIContent GetErrorMessageGUIContent(Type referenceType, Type interfaceType)
        {
            return new GUIContent(GetErrorMessage(referenceType, interfaceType));
        }

        static float GetHelpBoxHeight(GUIContent guiContent)
        {
            return EditorStyles.helpBox.CalcHeight(guiContent, EditorGUIUtility.currentViewWidth - 40f);
        }

        bool IsValueValid(UnityEngine.Object objectReferenceValue)
        {
            SerializedInterfaceAttribute serializedInterfaceAttribute = attribute as SerializedInterfaceAttribute;            
            if(objectReferenceValue == null)
                return true;
            return serializedInterfaceAttribute.interfaceType.IsAssignableFrom(objectReferenceValue.GetType());
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty referenceProperty = property.FindPropertyRelative("_value"); 
            if(!IsValueValid(referenceProperty.objectReferenceValue))
            {
                SerializedInterfaceAttribute serializedInterfaceAttribute = attribute as SerializedInterfaceAttribute;                
                GUIContent guiContent = GetErrorMessageGUIContent(referenceProperty.objectReferenceValue.GetType(), serializedInterfaceAttribute.interfaceType);                                
                return base.GetPropertyHeight(property, label) + GetHelpBoxHeight(guiContent) + 2f;
            }
            
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedInterfaceAttribute serializedInterfaceAttribute = attribute as SerializedInterfaceAttribute;
            SerializedProperty referenceProperty = property.FindPropertyRelative("_value"); 
            UnityEngine.Object objectReferenceValue = referenceProperty.objectReferenceValue;
            float objectFieldHeight = base.GetPropertyHeight(property, label);
            bool isValueValid = IsValueValid(referenceProperty.objectReferenceValue);
            Rect objectFieldPosition = new Rect(position.x, position.y, position.width, objectFieldHeight);
            
            EditorGUI.BeginChangeCheck();            
            EditorGUI.showMixedValue = referenceProperty.hasMultipleDifferentValues;
            UnityEngine.Object referencePropertyObjectReferenceValue = EditorGUI.ObjectField(objectFieldPosition, label,
                objectReferenceValue, typeof(UnityEngine.Object), serializedInterfaceAttribute.sceneObjects);
            
            if(!isValueValid)
            {
                GUIContent guiContent = GetErrorMessageGUIContent(referenceProperty.objectReferenceValue.GetType(), serializedInterfaceAttribute.interfaceType);
                float height = GetHelpBoxHeight(guiContent);
                EditorGUI.HelpBox(new Rect(position.x, position.y + objectFieldPosition.height + 2, position.width, height), guiContent.text, MessageType.Error);
            }

            if (EditorGUI.EndChangeCheck())
            {
                referenceProperty.objectReferenceValue = referencePropertyObjectReferenceValue;
            }
        }
    }
}