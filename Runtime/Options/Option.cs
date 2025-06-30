using UnityEngine;

public class Option : ScriptableObject
{
    [HideInInspector][SerializeField] private Options _options;
    public Options Options => _options;
}
