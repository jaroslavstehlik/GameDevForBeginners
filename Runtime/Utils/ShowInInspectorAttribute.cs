using UnityEngine;

public class ShowInInspectorAttribute : PropertyAttribute
{
    public bool editable = false;
    public ShowInInspectorAttribute(bool editable)
    {
        this.editable = editable;
    }
}