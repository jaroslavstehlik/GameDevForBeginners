using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Options))]
public class OptionsEditor : Editor
{
    private const string OPTIONS = "_options";
    
    private ReorderableList list;
    private SerializedProperty optionsProperty;

    private void OnEnable()
    {
        optionsProperty = serializedObject.FindProperty(OPTIONS);
        list = new ReorderableList(serializedObject, optionsProperty, true, true, true, true);
        list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Options"); };
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            serializedObject.Update();
            var optionProperty = optionsProperty.GetArrayElementAtIndex(index);
            var option = optionProperty.objectReferenceValue as Option;
            
            if (option != null)
            {
                EditorGUI.BeginChangeCheck();
                string newName = EditorGUI.TextField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    option.name
                );
                if (EditorGUI.EndChangeCheck())
                {
                    option.name = newName;
                    EditorUtility.SetDirty(option);
                    AssetDatabase.SaveAssets();
                }
            }
            else
            {
                EditorGUI.LabelField(rect, "Null");
            }
        };

        list.onAddCallback = (ReorderableList l) =>
        {
            SerializedProperty serializedProperty = l.serializedProperty;
            SerializedObject serializedObject = serializedProperty.serializedObject;
            serializedObject.Update();
            
            int itemIndex = serializedProperty.arraySize; 
            serializedProperty.arraySize++;
            
            Option newOption = CreateInstance<Option>();
            newOption.name = "New Option";
            
            SerializedObject newOptionSo = new SerializedObject(newOption);
            newOptionSo.FindProperty(OPTIONS).objectReferenceValue = serializedObject.targetObject;
            newOptionSo.ApplyModifiedPropertiesWithoutUndo();
            
            AssetDatabase.AddObjectToAsset(newOption, serializedObject.targetObject);
            AssetDatabase.SaveAssets();
            
            serializedProperty.GetArrayElementAtIndex(itemIndex).objectReferenceValue = newOption;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        };

        list.onRemoveCallback = (ReorderableList l) =>
        {
            SerializedProperty serializedProperty = l.serializedProperty;
            SerializedObject serializedObject = serializedProperty.serializedObject;
            serializedObject.Update();
            
            SortedDictionary<int, Object> listDictionary = new SortedDictionary<int, Object>();
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                listDictionary.Add(i, serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue);
            }
            
            foreach (var index in l.selectedIndices)
            {
                DestroyImmediate(listDictionary[index], true);
                listDictionary.Remove(index);
            }

            Object[] references = listDictionary.Values.ToArray();
            serializedProperty.arraySize = references.Length;
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue = references[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
        };
        
        list.onReorderCallback = (ReorderableList l) => { serializedObject.ApplyModifiedPropertiesWithoutUndo(); };
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }
}
