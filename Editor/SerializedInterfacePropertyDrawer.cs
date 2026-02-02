using System;
using UnityEditor;
using UnityEngine;

namespace GameDevForBeginners
{
    [CustomPropertyDrawer(typeof(SerializedInterfaceAttribute))]
    public class SerializedInterfacePropertyDrawer : PropertyDrawer
    {
        static int GetTypeIndex(Type[] types, Type type)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == type)
                {
                    return i;
                }
            }

            return -1;
        }

        static (UnityEngine.Object, int) GetObjectAndTypeIndex(UnityEngine.Object target, Type[] types)
        {
            GameObject draggedGameObject = target as GameObject;

            if (draggedGameObject != null)
            {
                MonoBehaviour[] components = draggedGameObject.GetComponents<MonoBehaviour>();
                foreach (var component in components)
                {
                    int index = GetTypeIndex(types, component.GetType());
                    if (index != -1)
                    {
                        return (component, index);
                    }
                }
            }

            return (target, GetTypeIndex(types, target.GetType()));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18 * 2;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float halfHeight = position.height * 0.5f;
            Rect topRect = new Rect(position.x, position.y, position.width, halfHeight);
            Rect bottomRect = new Rect(position.x, position.y + halfHeight, position.width, halfHeight);
            
            SerializedInterfaceAttribute serializedInterfaceAttribute = attribute as SerializedInterfaceAttribute;
            Type[] baseTypes = serializedInterfaceAttribute.baseTypes;
            
            string[] choices = new string[baseTypes.Length];
            for(int i = 0; i < choices.Length; i++)
            {
                choices[i] = baseTypes[i].Name;
            }
            
            SerializedProperty selectedTypeIndex = property.FindPropertyRelative("_selectedTypeIndex");
            SerializedProperty referenceProperty = property.FindPropertyRelative("_value");
            UnityEngine.Object objectReferenceValue = referenceProperty.objectReferenceValue;
            string labelText = label.text;
            string labelPopup = $"{labelText} type";
            
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = selectedTypeIndex.hasMultipleDifferentValues;
            int selectedTypeIndexIntValue = EditorGUI.Popup(topRect, labelPopup,
                Math.Clamp(selectedTypeIndex.intValue, 0, baseTypes.Length), choices);
            if (EditorGUI.EndChangeCheck())
            {
                selectedTypeIndex.intValue = selectedTypeIndexIntValue;
            }
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = referenceProperty.hasMultipleDifferentValues;
            UnityEngine.Object referencePropertyObjectReferenceValue = EditorGUI.ObjectField(bottomRect, $"{labelText} reference",
                objectReferenceValue, baseTypes[selectedTypeIndex.intValue]);
            if (EditorGUI.EndChangeCheck())
            {
                referenceProperty.objectReferenceValue = referencePropertyObjectReferenceValue;
            }
        }
    }
}