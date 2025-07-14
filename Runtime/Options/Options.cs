using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Options", menuName = "GMD/Options/Options", order = 1)]
public class Options : ScriptableObject
{
    [HideInInspector]
    [SerializeField]
    private Option[] _options = Array.Empty<Option>();
    public Option[] options => _options;

    public string[] optionNames
    {
        get
        {
            string[] optionNames = new string[_options.Length];
            for (int i = 0; i < optionNames.Length; i++)
            {
                optionNames[i] = _options[i].name;
            }
            return optionNames;
        }
    }

    public int GetOptionIndex(Option option)
    {
        for (int i = 0; i < _options.Length; i++)
        {
            if (_options[i] == option)
                return i;
        }

        return -1;
    }
}