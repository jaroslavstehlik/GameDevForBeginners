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
        bool inPlayMode = EditorApplication.isPlaying;
            
        foreach( (FieldInfo, ShowInInspector) f in fieldsToDraw )
        {
            FieldInfo fieldInfo = f.Item1;
            var fieldType = fieldInfo.FieldType;
            ShowInInspector showInInspector = f.Item2;
            
            bool editable = inPlayMode ? showInInspector.editableInPlayMode : showInInspector.editableInEditMode;
            EditorGUI.BeginDisabledGroup(!editable);
            var fieldName = ObjectNames.NicifyVariableName( fieldInfo.Name );
            var fieldLabel = new GUIContent( fieldName );
            
            var fieldValue = f.Item1.GetValue( obj );
            if (fieldType.BaseType == typeof( ScriptableObject ) ) {
                var val = (ScriptableObject)fieldValue;
                EditorGUI.BeginChangeCheck();
                val = EditorGUI.ObjectField( position, fieldLabel, val, typeof(ScriptableObject), false ) as ScriptableObject;
                if( EditorGUI.EndChangeCheck() ) {
                    fieldInfo.SetValue( obj, val );
                }
            } else if( fieldType == typeof( float ) ) {
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

    private static List<(FieldInfo, ShowInInspector)> GetDrawnFields ( SerializedProperty property ) {
        
        var targetType = property.serializedObject.targetObject.GetType();
        System.Type[] types;
        if (targetType != targetType.BaseType)
        {
            types = new System.Type[]{targetType, targetType.BaseType};
        }
        else
        {
            types = new System.Type[]{targetType};
        }
        
        var fieldsToDraw = new List<(FieldInfo, ShowInInspector)>();
        bool inPlayMode = EditorApplication.isPlaying;
        
        foreach (System.Type type in types)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var f in fields)
            {
                ShowInInspectorAttribute[] attr =
                    (ShowInInspectorAttribute[])f.GetCustomAttributes(typeof(ShowInInspectorAttribute), true);
                if (attr.Length > 0)
                {
                    ShowInInspector showInInspector = attr[0].showInInspector;
                    bool show = inPlayMode ? showInInspector.showInPlayMode : showInInspector.showInEditMode;
                    if(!show)
                        continue;

                    fieldsToDraw.Add((f, showInInspector));
                }
            }
        }

        return fieldsToDraw;
    }
}