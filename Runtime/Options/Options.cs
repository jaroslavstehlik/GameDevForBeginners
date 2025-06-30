using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Options", menuName = "GMD/Options/Options", order = 1)]
public class Options : ScriptableObject
{
    [HideInInspector]
    [SerializeField]
    private Option[] _options = Array.Empty<Option>();
    public Option[] options => _options;
}