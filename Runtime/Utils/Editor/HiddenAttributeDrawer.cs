using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawHiddenFieldsAttribute))]
public class HiddenDrawer : PropertyDrawer {

    public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label ) {
        var fieldsToDraw = GetDrawnFields( property );

        var obj = property.serializedObject.targetObject;
        position.height = EditorGUIUtility.singleLineHeight;
        
        foreach( (FieldInfo, bool) f in fieldsToDraw )
        {
            FieldInfo fieldInfo = f.Item1;
            var fieldType = fieldInfo.FieldType;
            bool editable = f.Item2;

            EditorGUI.BeginDisabledGroup(!editable);
            var fieldName = ObjectNames.NicifyVariableName( fieldInfo.Name );
            var fieldLabel = new GUIContent( fieldName );

            var fieldValue = f.Item1.GetValue( obj );

            if( fieldType == typeof( float ) ) {
                var val = (float)fieldValue;
                EditorGUI.BeginChangeCheck();
                val = EditorGUI.FloatField( position, fieldLabel, val );
                if( EditorGUI.EndChangeCheck() ) {
                    f.Item1.SetValue( obj, val );
                }
            } else if( fieldType == typeof( string ) ) {
                var val = (string)fieldValue;
                EditorGUI.BeginChangeCheck();
                val = EditorGUI.TextField( position, fieldLabel, val );
                if( EditorGUI.EndChangeCheck() ) {
                    fieldInfo.SetValue( obj, val );
                }
            } else if( fieldType == typeof( int ) ) {
                var val = (int)fieldValue;
                EditorGUI.BeginChangeCheck();
                val = EditorGUI.IntField( position, fieldLabel, val );
                if( EditorGUI.EndChangeCheck() ) {
                    fieldInfo.SetValue( obj, val );
                }
            } else if( fieldType == typeof( bool ) ) {
                var val = (bool)fieldValue;
                EditorGUI.BeginChangeCheck();
                val = EditorGUI.Toggle( position, fieldLabel, val );
                if( EditorGUI.EndChangeCheck() ) {
                    fieldInfo.SetValue( obj, val );
                }
            }
            position.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.EndDisabledGroup();
        }
    }

    public override float GetPropertyHeight ( SerializedProperty property, GUIContent label ) {
        var fieldsToDraw = GetDrawnFields( property );

        return EditorGUIUtility.singleLineHeight * fieldsToDraw.Count + 2 * ( fieldsToDraw.Count - 1 );
    }

    private static List<(FieldInfo, bool)> GetDrawnFields ( SerializedProperty property ) {
        
        var targetType = property.serializedObject.targetObject.GetType();
        Type[] types;
        if (targetType != targetType.BaseType)
        {
            types = new Type[]{targetType, targetType.BaseType};
        }
        else
        {
            types = new Type[]{targetType};
        }
        
        var fieldsToDraw = new List<(FieldInfo, bool)>();
        
        foreach (Type type in types)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var f in fields)
            {
                ShowInInspectorAttribute[] attr =
                    (ShowInInspectorAttribute[])f.GetCustomAttributes(typeof(ShowInInspectorAttribute), true);
                if (attr.Length > 0)
                {
                    fieldsToDraw.Add((f, attr[0].editable));
                }
            }
        }

        return fieldsToDraw;
    }
}