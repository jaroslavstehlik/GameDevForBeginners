using UnityEngine;

public struct ShowInInspector
{
    public bool showInEditMode;
    public bool editableInEditMode;
    public bool showInPlayMode;
    public bool editableInPlayMode;
}

public class ShowInInspectorAttribute : PropertyAttribute
{
    public ShowInInspector showInInspector;
    public ShowInInspectorAttribute(bool showInEditMode, bool editableInEditMode, bool showInPlayMode, bool editableInPlayMode)
    {
        showInInspector = new ShowInInspector()
        {
            showInEditMode = showInEditMode,
            editableInEditMode = editableInEditMode,
            showInPlayMode = showInPlayMode,
            editableInPlayMode = editableInPlayMode,
        };
    }
}