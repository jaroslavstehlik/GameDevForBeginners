using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TagAttribute))]
public class TagAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
        EditorGUI.EndProperty();
    }
}