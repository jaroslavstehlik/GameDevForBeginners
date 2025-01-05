using UnityEditor;
using UnityEngine;

namespace GameDevForBeginners
{
    enum StatePropertyType
    {
        Unknown,
        State,
        StateDescriptor
    }
    
    [CustomPropertyDrawer(typeof(StateAttribute))]
    public class StateAttributeDrawer : PropertyDrawer
    {
        static void RenderProperty(Rect position, SerializedProperty property)
        {
            EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
        }

        SerializedProperty FindStateProperty(SerializedProperty property, out StatePropertyType statePropertyType)
        {
            SerializedProperty serializedProperty = null;
            
            serializedProperty = property.serializedObject.FindProperty("_state");
            if (serializedProperty != null)
            {
                statePropertyType = StatePropertyType.State;
                return serializedProperty;
            }

            serializedProperty = property.serializedObject.FindProperty("_stateDescriptor");
            if (serializedProperty != null)
            {
                statePropertyType = StatePropertyType.StateDescriptor;
                return serializedProperty;
            }

            statePropertyType = StatePropertyType.Unknown;
            return serializedProperty;
        }
        
        string[] FindOptions(SerializedProperty property)
        {
            if (property.serializedObject.targetObject as IState != null)
            {
                SerializedProperty statesProperty = property.serializedObject.FindProperty("_states");
                if (statesProperty != null)
                {
                    string[] options = new string[statesProperty.arraySize];
                    for (int i = 0; i < options.Length; i++)
                    {
                        options[i] = statesProperty.GetArrayElementAtIndex(i).stringValue;
                    }

                    return options;
                }
            }
            
            SerializedProperty stateProperty = FindStateProperty(property, out StatePropertyType statePropertyType);
            if (stateProperty != null)
            {
                switch (statePropertyType)
                {
                    case StatePropertyType.State:
                    {
                        IState state = stateProperty.objectReferenceValue as IState;
                        if (state != null)
                        {
                            return (string[])state.states.Clone();
                        }
                        break;
                    }
                    case StatePropertyType.StateDescriptor:
                    {
                        SerializedProperty statesProperty = stateProperty.FindPropertyRelative("states");
                        if (statesProperty != null)
                        {
                            string[] options = new string[statesProperty.arraySize];
                            for (int i = 0; i < options.Length; i++)
                            {
                                options[i] = statesProperty.GetArrayElementAtIndex(i).stringValue;
                            }

                            return options;
                        }
                        break;
                    }
                }
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